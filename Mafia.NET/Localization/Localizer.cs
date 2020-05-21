using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
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

        public Catalog Get([CanBeNull] CultureInfo culture = null)
        {
            culture ??= _defaultCulture;

            if (!_catalogs.ContainsKey(culture))
                _catalogs[culture] = Load(culture);

            return _catalogs[culture];
        }

        public string Get(Key key, [CanBeNull] CultureInfo culture = null)
        {
            culture ??= _defaultCulture;

            var catalog = Get(culture);
            var defaultString = catalog.GetStringDefault(key.Id, null);
            if (defaultString == null) throw new ArgumentException($"Key {key.Id} not found for culture {culture}");

            var str = catalog.GetStringDefault(key.Id, defaultString);

            return str;
        }

        public Text Get(Notification notification, [CanBeNull] CultureInfo culture = null)
        {
            culture ??= _defaultCulture;

            var key = notification.Key;
            var catalog = Get(culture);
            var defaultString = catalog.GetStringDefault(key, null);
            if (defaultString == null) throw new ArgumentException($"Key {key} not found for culture {culture}");

            var str = catalog.GetStringDefault(key, defaultString);

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