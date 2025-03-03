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
        public NewsTitleModel TitleModel { get; set; }
        public string FullContent { get; set;}

        public override string ToString()
        {
            return "result: \n\n" + 
                TitleModel.Title + "\n\tPublishDate: " +
                TitleModel.PublishDate + ";\n\tPreviews: " +
                TitleModel.ViewsCount + "\n\ttags: " +
                TitleModel.TagList + "\n\tFileName: " +
                TitleModel.FileName;
        }
    }
}
