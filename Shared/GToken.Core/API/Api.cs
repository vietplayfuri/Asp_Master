namespace Platform.Core
{
    public partial class Api
    {
        
        #region Singleton
        private Api() { }

        public static readonly Api Instance;

        static Api()
        {
            Instance = new Api();
        }
        #endregion
    }
}
