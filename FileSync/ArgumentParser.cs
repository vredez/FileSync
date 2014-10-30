using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    /// <summary>
    /// Class parsing commandline arguments.
    /// </summary>
    public class ArgumentParser
    {
        Dictionary<string, List<string>> arguments;
        List<string> keyless;
        string[] nullList = new string[] { };

        /// <summary>
        /// Gets the arguments of a key.
        /// </summary>
        public IReadOnlyList<string> this[string key]
        {
            get
            {
                if (arguments.ContainsKey(key))
                {
                    return arguments[key];
                }

                return nullList;
            }
        }

        /// <summary>
        /// Gets the keyless arguments.
        /// </summary>
        public IReadOnlyList<string> Keyless
        {
            get
            {
                return keyless;
            }
        }

        /// <summary>
        /// Gets all available keys.
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                return arguments.Keys;
            }
        }

        public ArgumentParser()
        {
            arguments = new Dictionary<string, List<string>>();
            keyless = new List<string>();

            List<string> currentArgs = null;
            string currentKey = null;

            var args = Environment.GetCommandLineArgs();

            for (int i = 1; i < args.Length; ++i )
            {
                var arg = args[i];

                if (arg.StartsWith("-"))
                {
                    var key = arg.TrimStart('-');

                    if (arguments.ContainsKey(key))
                    {
                        currentArgs = arguments[key];
                    }
                    else
                    {
                        currentArgs = new List<string>();
                        arguments.Add(key, currentArgs);
                    }

                    currentKey = key;
                }
                else
                {
                    if (currentArgs != null)
                    {
                        currentArgs.Add(arg);
                    }
                    else
                    {
                        keyless.Add(arg);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether a key exists in the arguments.
        /// </summary>
        public bool KeyExists(string key)
        {
            return arguments.ContainsKey(key);
        }
    }
}
