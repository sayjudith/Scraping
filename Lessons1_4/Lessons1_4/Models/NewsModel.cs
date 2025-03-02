using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lessons1_4.Models
{
    internal class NewsTitleModel
    {
        public string Title { get; set; }
        public string PublishDate { get; set; }
        public string ViewsCount { get; set; }
        public string TagList { get; set; }
        public string FileName { get; set; }
        public string LinkReference { get; set; }
        public override string ToString()
        {
            return "result: \n\n" + Title + "\n\tPublishDate: " + PublishDate + ";\n\tPreviews: " + ViewsCount + "\n\ttags: " + TagList + "\n\tFileName: " + FileName;
        }
    }

    internal class NewsModel
    {
        public NewsTitleModel titleModel { get; set; }
        public string FullContent { get; set;}

        public override string ToString()
        {
            return "result: \n\n" + 
                titleModel.Title + "\n\tPublishDate: " +
                titleModel.PublishDate + ";\n\tPreviews: " +
                titleModel.ViewsCount + "\n\ttags: " +
                titleModel.TagList + "\n\tFileName: " +
                titleModel.FileName;
        }
    }
}
