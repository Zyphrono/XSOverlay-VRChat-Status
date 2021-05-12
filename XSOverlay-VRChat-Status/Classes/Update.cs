using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onova;
using Onova.Exceptions;
using Onova.Services;

namespace XSOverlay_VRChat_Status.Classes
{
    class Update
    {
        private readonly IUpdateManager _updateManager = new UpdateManager(
            new GithubPackageResolver("KnuffelBeestje", "XSOverlay-VRChat-Status", "XSOVR-VRCStatus.zip"),
            new ZipPackageExtractor()
        );
        private Version _updateVersion;
        private bool _updatePrepared;
        private bool _updaterLaunched;
        private Version _lastVersion;

        public Version LatestAvailableVersion
        {
            get { return _lastVersion; }
        }
        public async Task<Version> CheckForUpdatesAsync()
        {
            var check = await _updateManager.CheckForUpdatesAsync();
            _lastVersion = check.LastVersion;
            return check.CanUpdate ? check.LastVersion : null;
        }

        public async Task PrepareUpdateAsync(Version version)
        {
            try
            {
                await _updateManager.PrepareUpdateAsync(_updateVersion = version);
                _updatePrepared = true;
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore race conditions
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore race conditions
            }
        }

        public void FinalizeUpdate(bool needRestart)
        {
            if ((_updateVersion is null) || (!_updatePrepared) || (_updaterLaunched))
                return;

            try
            {
                _updateManager.LaunchUpdater(_updateVersion, needRestart);
                _updaterLaunched = true;
                Environment.Exit(0);
            }
            catch (UpdaterAlreadyLaunchedException)
            {
                // Ignore
            }
            catch (LockFileNotAcquiredException)
            {
                // Ignore
            }
        }
        public void Dispose() => _updateManager.Dispose();
    }
}
