using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iguana.IguanaMesh.IWrappers.IExtensions
{
    public class IguanaGmshFieldCollection
    {
        private List<IguanaGmshField> fields;

        public IguanaGmshFieldCollection()
        {
            fields = new List<IguanaGmshField>();
        }

        public void AddField(IguanaGmshField f)
        {
            fields.Add(f);
        }

        public int Count { get => fields.Count; }

        public void ApplyFields()
        {
            foreach (IguanaGmshField f in fields)
            {
                f.ApplyField();
                IguanaGmsh.Model.MeshField.SetAsBackgroundMesh(f.Tag);
            }
        }
    }
}
