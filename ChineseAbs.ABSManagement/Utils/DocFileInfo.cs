namespace ChineseAbs.ABSManagement.Utils
{
    public class DocFileInfo
    {
        public DocFileInfo()
        {
        }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string Extension
        {
            get { return m_extension; }
            set
            {
                m_extension = value;
                m_mime = FileUtils.GetMIME(Extension);
            }
        }
        private string m_extension;

        /// <summary>
        /// MIME类型
        /// </summary>
        public string MIME
        {
            get { return m_mime; }
        }

        private string m_mime;

        /// <summary>
        /// 绝对路径
        /// </summary>
        public string AbsultePath
        {
            get { return m_absultePath; }
            set
            {
                m_absultePath = value;
                LogicName = value;
            }
        }
        private string m_absultePath;

        /// <summary>
        /// 逻辑文件名（文件系统中的文件名）
        /// </summary>
        public string LogicName
        {
            get
            {
                return FileUtils.CombineExtension(LogicNameWithoutExtension, m_extension);
            }

            set
            {
                Extension = FileUtils.GetExtension(value);
                LogicNameWithoutExtension = FileUtils.GetFileNameWithoutExtension(value);
            }
        }

        /// <summary>
        /// 显示文件名（上传下载时的文件名）
        /// </summary>
        public string DisplayName
        {
            get
            {
                return FileUtils.CombineExtension(DisplayNameWithoutExtension, m_extension);
            }

            set
            {
                Extension = FileUtils.GetExtension(value);
                DisplayNameWithoutExtension = FileUtils.GetFileNameWithoutExtension(value);
            }
        }

        /// <summary>
        /// 不含扩展名的逻辑文件名（文件系统中的文件名）
        /// </summary>
        public string LogicNameWithoutExtension { get; set; }

        /// <summary>
        /// 不含扩展名的显示文件名（上传下载时的文件名）
        /// </summary>
        public string DisplayNameWithoutExtension { get; set; }
    }
}
