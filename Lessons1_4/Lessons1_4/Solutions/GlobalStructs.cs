

namespace Lessons1_4.Solutions
{
    internal struct NewsStruct
    {
        private string sTitle;
        private string sPublishDate;
        private string sViewsCount;
        private string sTagsList;
        private string sFullContent;
        private string sFileName;

        public string Title {  get { return sTitle; } set { sTitle = value; } }
        public string PublishDate { get { return sPublishDate; } set {sPublishDate = value; } }
        public string ViewsCount { get { return sViewsCount; } set { sViewsCount = value; } }
        public string TagList { get { return sTagsList; } set { sTagsList = value; } }
        public string FullContent { get { return sFullContent; } set { if (sFullContent != value) { sFullContent = value; } } }
        public string FileName { get { return sFileName; } set { sFileName = value; } }

        public override string ToString()
        {
            return "result: \n\n" + this.Title + "\n\tPublishDate: " + PublishDate + ";\n\tPreviews: " + ViewsCount + "\n\ttags: " + TagList + "\n\tFileName: " + FileName;
        }

    }
}
