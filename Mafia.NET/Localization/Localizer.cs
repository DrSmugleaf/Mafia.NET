using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Mafia.NET.Extension;
using Mafia.NET.Notifications;
using Mafia.NET.Resources;
using NGettext;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Localization
{
    public class Localizer
    {
        private static readonly Lazy<Localizer> Lazy = new Lazy<Localizer>(() => new Localizer());

        private readonly Dictionary<CultureInfo, Catalog> _catalogs;
        private readonly CultureInfo _defaultCulture;
        private readonly Parser _parser;

        private Localizer()
        {
            _catalogs = new Dictionary<CultureInfo, Catalog>();
            _parser = new Parser();
            _defaultCulture = CultureInfo.CreateSpecificCulture("en_US");
        }

        public static Localizer Default => Lazy.Value;

        public Catalog Get(CultureInfo culture)
        {
            if (!_catalogs.ContainsKey(culture))
                _catalogs[culture] = Load(culture);

            return _catalogs[culture];
        }

        public string Get(Key key, CultureInfo culture)
        {
            var defaultString = Get(culture).GetString(key.Id);
            var str = _catalogs[culture].GetStringDefault(key.Id, defaultString);

            return str;
        }

        public Text Get(Notification notification, CultureInfo culture)
        {
            var key = notification.Key.ToLower();
            var defaultString = Get(culture).GetString(key);
            var str = _catalogs[culture].GetStringDefault(key, defaultString);

            return _parser.Parse(str, notification.Location, notification.Args);
        }

        private Catalog Load(CultureInfo culture)
        {
            var catalog = new Catalog(culture);
            var files = Resource.FromDirectory("Locale/" + culture.IetfLanguageTag, "*.yml");
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.ResourcePath);
                foreach (var pair in ((YamlMappingNode) file).Children)
                {
                    var key = (fileName + pair.Key.AsString()).ToLower();
                    var value = pair.Value.AsString();

                    if (catalog.Translations.ContainsKey(key))
                        throw new ArgumentException($"Key {key} already exists in catalog {catalog}");

                    catalog.Translations.Add(key, new[] {value});
                }
            }

            return catalog;
        }
    }
}