using Galssoft.VKontakteWM.Components.Common.ResponseClasses;
using Galssoft.VKontakteWM.Components.ResponseClasses;
using System;

namespace Galssoft.VKontakteWM.Components.Data
{
    public interface IDataLogic
    {
        byte[] GetCSPBlobValue();
        byte[] GetSavedCryptoPass();
        string GetToken();
        string GetUid();
        string GetSavedLogin();
        string GetSessionKey();
        string GetPhotoRHash();
        string GetPhotoHash();
        string GetPhotoUploadAddress();
        string GetAid();
        string GetAvatarRHash();
        string GetAvatarHash();
        string GetAvatarUploadAddress();
        string GetSessionSecretKey();
        string GetNotificationTimer();
        RawEventsGetResponse GetEvents();
        EventsGetResponse EventsGet();
        string GetRefreshEventsFlag();
        Version GetLastCheckedVersion();
        string GetLastCheckedVersionStr();
        string GetLastCheckedVersionInfo();
        bool GetLastCheckedVersionIsMandatory();
        Version GetLastCheckedVersionIsSkipped();
        string GetLastCheckedVersionUpdateURL();
        bool GetTraceMessages();
        bool GetTraceComments();
        bool GetTraceFriends();
        bool GetTraceFriendsPhotos();
        bool GetTraceFriendsNews();
        bool GetTraceWallMessages();
        bool GetUplPhtViewHasMdfPht();
        OpenNETCF.Drawing.RotationAngle GetUplPhtViewPhtRtnAngl();
        string GetUplPhtViewPhtCmnt();
        string GetUpdateValue();
        string GetLoadPhotoValue();
        string GetInRoumingValue();
        bool GetNtfAutorun();
        string GetDownloadAtStart();
        string GetUpdateFriendsStatus();
        string GetImageMaxSize();
        string GetBackgroundNotification();
        string GetDataRenewType();
        bool GetShowButtonMessages();
        bool GetShowButtonComments();
        bool GetShowButtonFriends();
        bool GetShowButtonFriendsNews();
        bool GetShowButtonFriendsPhotos();
        bool GetShowButtonWallMessages();
        string GetOnlyWIFI();

        void SetCSPBlobValue(byte[] val);
        void SetSavedCryptoPass(byte[] val);
        void SetUid(string val);
        void SetSessionKey(string val);
        void SetSessionSecretKey(string val);
        void SetPhotoHash(string val);
        void SetPhotoRHash(string val);
        void SetPhotoUploadAddress(string val);
        void SetAid(string val);
        void SetAvatarHash(string val);
        void SetAvatarRHash(string val);
        void SetAvatarUploadAddress(string val);
        void SetToken(string val);
        void SetEvents(RawEventsGetResponse eventsGetResponse);
        void SetNotificationTimer(string val);
        void SetSavedLogin(string val);
        void SetRefreshEventsFlag(string val);
        void SetLastCheckedVersion(Version val);
        void SetLastCheckedVersionInfo(string val);
        void SetLastCheckedVersionIsMandatory(bool val);
        void SetLastCheckedVersionIsSkipped(Version val);
        void SetLastCheckedVersionUpdateURL(string val);
        void SetTraceMessages();
        void SetTraceComments();
        void SetTraceFriends();
        void SetTraceFriendsPhotos();
        void SetTraceFriendsNews();
        void SetTraceWallMessages();
        void SetUplPhtViewHasMdfPht(bool val);
        void SetUplPhtViewPhtRtnAnglCW();
        void SetUplPhtViewPhtRtnAnglCCW();
        void SetUplPhtViewPhtRtnAnglZero();
        void SetUntraceComments();
        void SetUntraceFriends();
        void SetUntraceFriendsNews();
        void SetUntraceFriendsPhotos();
        void SetUntraceWallMessages();

        void SetShowButtonMessages();
        void SetShowButtonComments();
        void SetShowButtonFriends();
        void SetShowButtonFriendsNews();
        void SetShowButtonFriendsPhotos();
        void SetShowButtonWallMessages();

        void SetUplPhtViewPhtCmnt(string val);
        void SetUpdateValue(string val);
        void SetLoadPhotoValue(string val);
        void SetInRoumingValue(string val);
        void SetDownloadAtStart(string val);
        void SetUpdateFriendsStatus(string val);
        void SetImageMaxSize(string val);
        void SetBackgroundNotification(string val);
        void SetDataRenewType(string val);
        void SetOnlyWIFI(string val);

        void ClearCache();
        void ClearPass();

        void DelNtfAutorun();
        void SetNtfAutorun();
    }
}
