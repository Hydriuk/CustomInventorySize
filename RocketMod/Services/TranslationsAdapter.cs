using CustomInventorySize.API;
using Rocket.API.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomInventorySize.RocketMod.Services
{
    public class TranslationsAdapter : ITranslationsAdapter
    {
        private readonly TranslationList _tranlsations;

        public TranslationsAdapter(TranslationList translations)
        {
            _tranlsations = translations;
        }

        public string this[string key] 
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                return _tranlsations[key] ?? key;
            }
        }

        public string this[string key, object arguments]
        {
            get 
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                string[] splittedText = _tranlsations[key].Split('{', '}');

                FieldInfo[] fields = arguments.GetType().GetFields();
                string[] values = new string[fields.Length];

                for (int i = 1; i < splittedText.Length; i += 2)
                {
                    FieldInfo info = fields.FirstOrDefault(field => field.Name == splittedText[i]);

                    if (info == null)
                        continue;

                    var index = (i - 1) / 2;

                    values[index] = info.GetValue(arguments).ToString();
                    splittedText[i] = $"{index}";
                }

                string format = string.Join(string.Empty, splittedText);

                return string.Format(format ?? key, values);
            }
        }
    }
}
