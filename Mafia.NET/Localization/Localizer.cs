using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using Mafia.NET.Extension;
using Mafia.NET.Resources;
using NGettext;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Localization
{
    public class Localizer
    {
        private static readonly Lazy<Localizer> Lazy = new Lazy<Localizer>(() => new Localizer());

        private Localizer()
        {
            Catalogs = new Dictionary<CultureInfo, Catalog>();
            Parser = new Parser();
            DefaultCulture = CultureInfo.CreateSpecificCulture("en_US");
        }

        public Dictionary<CultureInfo, Catalog> Catalogs { get; }
        public CultureInfo DefaultCulture { get; }
        public Parser Parser { get; }

        public static Localizer Default => Lazy.Value;

        public Catalog Get([CanBeNull] CultureInfo culture = null)
        {
            culture ??= DefaultCulture;

            if (!Catalogs.ContainsKey(culture))
                Catalogs[culture] = Load(culture);

            return Catalogs[culture];
        }

        public Text Get(string key, [CanBeNull] CultureInfo culture = null, params object[] args)
        {
            if (key.Length == 0) return Text.Empty;
            culture ??= DefaultCulture;

            var catalog = Get(culture);
            var defaultString = catalog.GetStringDefault(key, null);
            if (defaultString == null) throw new ArgumentException($"Key {key} not found for culture {culture}");

            var str = catalog.GetStringDefault(key, defaultString);
            return Parser.Parse(str, args);
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