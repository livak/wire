using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;

namespace Wire.Infrastructure
{
    [DataContract]
    public abstract class ViewModeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        public void NotifyAllPropertiesChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        private List<string> states = new List<string>();
        private int currentStateIndex = -1;
        public void SaveState()
        {
            var state = ToXml();
            if (currentStateIndex < 0 || state != states[currentStateIndex])
            {
                var nextIndex = currentStateIndex + 1;
                states.RemoveRange(nextIndex, states.Count - nextIndex);
                states.Add(state);
                currentStateIndex = states.Count - 1;
            }
        }

        public void Undo()
        {
            if (CanUndo())
            {
               FromXml(states[--currentStateIndex]);
            }
        }

        public bool CanUndo()
        {
            return currentStateIndex > 0;
        }

        public void Redo()
        {
            if (CanRedo())
            {
                FromXml(states[++currentStateIndex]);
            }
        }

        public bool CanRedo()
        {
            var lastIndex = states.Count - 1;

            return currentStateIndex < lastIndex;
        }

        public string ToXml()
        {
            var serializer = new DataContractSerializer(this.GetType());
            using (var output = new StringWriter())
            using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
            {
                serializer.WriteObject(writer, this);
                return output.GetStringBuilder().ToString();
            }
        }

        public void FromXml(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return;
            }
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                DataContractSerializer serializer = new DataContractSerializer(this.GetType());
                var obj = serializer.ReadObject(reader);
                MergeDataMembers(from: obj, to: this);
            }
        }

        private static void MergeDataMembers(object from, object to)
        {
            foreach (var property in from.GetType().GetProperties())
            {
                if (property.GetCustomAttributes(typeof(DataMemberAttribute), false).GetLength(0) == 1)
                {
                    property.SetValue(to, property.GetValue(from, null), null);
                }
            }
        }
    }
}
