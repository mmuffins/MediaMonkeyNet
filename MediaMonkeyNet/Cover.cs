using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMonkeyNet
{
    public class Cover
    {
        public Cover(EvaluateResponse<IEnumerable<EvaluateObjectProperty<object>>> obj)
        {
            this.CoverStorage = int.Parse(obj.Value.Where(x => x.Name == "coverStorage").FirstOrDefault().Value.ToString());
            this.CoverType = int.Parse(obj.Value.Where(x => x.Name == "coverType").FirstOrDefault().Value.ToString());
            this.CoverTypeDesc = obj.Value.Where(x => x.Name == "coverTypeDesc").FirstOrDefault().Value.ToString();
            this.Deleted = bool.Parse(obj.Value.Where(x => x.Name == "deleted").FirstOrDefault().Value.ToString());
            this.Description = obj.Value.Where(x => x.Name == "description").FirstOrDefault().Value.ToString();
            this.Id = int.Parse(obj.Value.Where(x => x.Name == "id").FirstOrDefault().Value.ToString());
            this.PersistentID = obj.Value.Where(x => x.Name == "persistentID").FirstOrDefault().Value.ToString();
            this.PicturePath = obj.Value.Where(x => x.Name == "picturePath").FirstOrDefault().Value.ToString();
            this.PictureType = obj.Value.Where(x => x.Name == "pictureType").FirstOrDefault().Value.ToString();
        }

        public int CoverStorage { get; private set; }

        public int CoverType { get; private set; }

        public string CoverTypeDesc { get; private set; }

        public bool Deleted { get; private set; }

        public string Description { get; private set; }

        public int Id { get; private set; }

        public string PersistentID { get; private set; }

        public string PicturePath { get; private set; }

        public string PictureType { get; private set; }
    }
}
