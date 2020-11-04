using System;

namespace ProvaSP.Model.Utils.Attributes
{
    public class PathAttribute : Attribute
    {
        public string Caminho { get; private set; }

        public PathAttribute(string caminho)
        {
            Caminho = caminho;
        }
    }
}