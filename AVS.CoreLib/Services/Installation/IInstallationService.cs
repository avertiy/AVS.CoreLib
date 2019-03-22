namespace AVS.CoreLib.Services.Installation
{
    /// <summary>
    /// The installation service is called when database is created by EfStartUpTaskBase
    /// </summary>
    public partial interface IInstallationService
    {
        void InstallScheduledTasks(bool reinitialize = false);
        void InstallData();
        void ClearData();
    }
}
