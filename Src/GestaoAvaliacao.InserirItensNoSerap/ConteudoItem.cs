using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InserirItensNoSerap
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ConteudoItem
    {

        private uint sequenceField;

        private ConteudoItemAreaDeConhecimento areaDeConhecimentoField;

        private ConteudoItemDisciplina disciplinaField;

        private ConteudoItemMatriz matrizField;

        private ConteudoItemKeywords keywordsField;

        private ConteudoItemItemCode itemCodeField;

        private string comandoField;

        private byte difField;

        private ConteudoItemHab habField;

        private ConteudoItemSubAssunto subAssuntoField;

        private ConteudoItemAno anoField;

        private string observacaoField;

        private ConteudoItemTextoBase textoBaseField;

        private ConteudoItemOpcao[] opcoesField;

        private string versaoField;

        /// <remarks/>
        public uint Sequence
        {
            get
            {
                return this.sequenceField;
            }
            set
            {
                this.sequenceField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemAreaDeConhecimento AreaDeConhecimento
        {
            get
            {
                return this.areaDeConhecimentoField;
            }
            set
            {
                this.areaDeConhecimentoField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemDisciplina Disciplina
        {
            get
            {
                return this.disciplinaField;
            }
            set
            {
                this.disciplinaField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemMatriz Matriz
        {
            get
            {
                return this.matrizField;
            }
            set
            {
                this.matrizField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemKeywords Keywords
        {
            get
            {
                return this.keywordsField;
            }
            set
            {
                this.keywordsField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemItemCode ItemCode
        {
            get
            {
                return this.itemCodeField;
            }
            set
            {
                this.itemCodeField = value;
            }
        }

        /// <remarks/>
        public string Comando
        {
            get
            {
                return this.comandoField;
            }
            set
            {
                this.comandoField = value;
            }
        }

        /// <remarks/>
        public byte Dif
        {
            get
            {
                return this.difField;
            }
            set
            {
                this.difField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemHab Hab
        {
            get
            {
                return this.habField;
            }
            set
            {
                this.habField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemSubAssunto SubAssunto
        {
            get
            {
                return this.subAssuntoField;
            }
            set
            {
                this.subAssuntoField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemAno Ano
        {
            get
            {
                return this.anoField;
            }
            set
            {
                this.anoField = value;
            }
        }

        /// <remarks/>
        public string Observacao
        {
            get
            {
                return this.observacaoField;
            }
            set
            {
                this.observacaoField = value;
            }
        }

        /// <remarks/>
        public ConteudoItemTextoBase TextoBase
        {
            get
            {
                return this.textoBaseField;
            }
            set
            {
                this.textoBaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Opcao", IsNullable = false)]
        public ConteudoItemOpcao[] Opcoes
        {
            get
            {
                return this.opcoesField;
            }
            set
            {
                this.opcoesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Versao
        {
            get
            {
                return this.versaoField;
            }
            set
            {
                this.versaoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemAreaDeConhecimento
    {

        private byte codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemDisciplina
    {

        private byte codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemMatriz
    {

        private byte codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemKeywords
    {

        private string codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemItemCode
    {

        private string codeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemHab
    {

        private ushort codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemSubAssunto
    {

        private byte codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemAno
    {

        private byte codeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemTextoBase
    {

        private string descricaoField;

        private string fonteField;

        /// <remarks/>
        public string Descricao
        {
            get
            {
                return this.descricaoField;
            }
            set
            {
                this.descricaoField = value;
            }
        }

        /// <remarks/>
        public string Fonte
        {
            get
            {
                return this.fonteField;
            }
            set
            {
                this.fonteField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ConteudoItemOpcao
    {

        private string enunciadoField;

        private string justificativaField;

        private bool corretoField;

        private string idOpcaoField;

        /// <remarks/>
        public string Enunciado
        {
            get
            {
                return this.enunciadoField;
            }
            set
            {
                this.enunciadoField = value;
            }
        }

        /// <remarks/>
        public string Justificativa
        {
            get
            {
                return this.justificativaField;
            }
            set
            {
                this.justificativaField = value;
            }
        }

        /// <remarks/>
        public bool Correto
        {
            get
            {
                return this.corretoField;
            }
            set
            {
                this.corretoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IdOpcao
        {
            get
            {
                return this.idOpcaoField;
            }
            set
            {
                this.idOpcaoField = value;
            }
        }
    }


}
