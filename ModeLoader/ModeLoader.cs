using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IModes;

namespace IModes
{
    public class ModeLoader
    {
        //Liste des modes trouves
        private List<IMode> lesmodes = new List<IMode>();
        private String path_ = null;

        public ModeLoader(String pathplugin)
        {
            path_ = pathplugin;
            LoadPlugins();
        }

        public List<IMode> getListMode()
        {
            return lesmodes;
        }

        private void LoadPlugins()
        {
            // search plugin directory for dlls
            string[] files = Directory.GetFiles(path_, "*.dll");

            // check each file in plugin direction
            foreach (string file in files)
            {
                try
                {
                    // load the assembly and get types
                    Assembly assembly = Assembly.LoadFrom(file); 
                    System.Type[] types = assembly.GetTypes();
                   
                    Console.WriteLine(file + " " + types.Count());

                    // look for our interface in the assembly
                    foreach (System.Type type in types)
                    {
                        // if we found our interface, verify attributes
                        if (type.BaseType != null && type.BaseType.Name == "IMode")
                        {
                            Boolean alreadyinclude = false;
                            // create the plugin using reflection
                            Object o = Activator.CreateInstance(type);
                                                
                            foreach (Object os in lesmodes)
                            {
                                if (((IMode)os).getModeName().Equals(((IMode)o).getModeName()))
                                    alreadyinclude = true;
                                else
                                    Console.WriteLine("Plugin already included" + ((IMode)o).getModeName());
                            }
                            if (!alreadyinclude)
                                lesmodes.Add((IMode)o);
                        }
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException e)
                {
                    for (int i = 0; i < e.LoaderExceptions.Count(); ++i)
                        Console.WriteLine(" Error : " + e.LoaderExceptions[i].Message);
                    Console.WriteLine("Plugin Error : " + e.Message);
                }
            }
        }
    }
}
