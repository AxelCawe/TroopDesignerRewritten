using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TroopDesignerRewritten.TroopDesignerRoster
{
    [XmlRoot("TroopDesignerRosterSettings")]
    public class TroopDesignerRosterSettings
    {
        [XmlElement("Character")]
        public List<TroopDesignerRosterEntry> Characters = new List<TroopDesignerRosterEntry>();
    }

    public class TroopDesignerRosterEntry
    {
        [XmlAttribute("id")]
        public string Id = "";
    }
}
