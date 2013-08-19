using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Entities.Helpers
{
    public static class NameHelper
    {

        static List<string> Names;
        private static int i = 0;

        private static int NextId = 0;

        public static string NextName
        {
            get
            {
                if (Names == null)
                {
                    Names = new List<string>();
                    Names.Add("1-Jack");
                    Names.Add("2-Joe");
                    Names.Add("3-Mary");
                    Names.Add("4-Jane");
                    Names.Add("5-Jim");
                    Names.Add("6-Jill");
                }
                
                return Names[i++ % Names.Count];
            }

        }

        public static int GetNextId()
        {
            return NextId++;
        }
    }
}
