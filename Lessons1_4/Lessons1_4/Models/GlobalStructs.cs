﻿

namespace Lessons1_4.Models
{
    internal struct NewsStruct
    {
        public string title;
        public string publishDate;
        public string viewsCount;
        public string tagsList;
        public string fullContent;
        public string fileName;

        public string Title { get; set; }
        public string PublishDate { get; set; }
        public string ViewsCount { get; set; }
        public string TagList { get; set; }
        public string FullContent { get { return fullContent; } set { if (fullContent != value) { fullContent = value; } } }
        public string FileName { get; set; }

        public override string ToString()
        {
            return "result: \n\n" + this.Title + "\n\tPublishDate: " + PublishDate + ";\n\tPreviews: " + ViewsCount + "\n\ttags: " + TagList + "\n\tFileName: " + FileName;
        }

    }
}
