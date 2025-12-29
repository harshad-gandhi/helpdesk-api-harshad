using System.ComponentModel;
using HelpDesk.Common.Constants;

namespace HelpDesk.Common.Enums
{
    public class Enumerations
    {

        #region Chat Status

        public enum ChatStatus
        {
            Active = 1,

            OnHoldByUser = 2,

            OnHoldByCustomer = 3,

            Missed = 4,

            Closed = 5,

            Transferred = 6,

            Abandoned = 7,
        }
        public enum Visibility
        {
            Public = 1,
            Private = 2
        }
        public enum Status
        {
            Draft = 1,
            Published = 2,
            Archived = 3
        }
        public enum Language
        {
            English = 1,
            Hindi = 2,
            Spanish = 3,
            French = 4
        }
        
        public enum ChatMessageType
        {
            Text = 1,
            File = 2,
            ChatStatusChange = 3,

        }

        public enum ChatMessageSenderType
        {
            Person = 1,
            Agent = 2,
            Bot = 3,
            System = 4,
        }

        #endregion

        #region Mime Type

        public enum MimeType
        {

            [Description(SystemConstant.UNKNOWN)]
            Unknown = 0,

            [Description(SystemConstant.JPEG)]
            Jpeg = 1,

            [Description(SystemConstant.PNG)]
            Png = 2,

            [Description(SystemConstant.PDF)]
            Pdf = 3,

            [Description(SystemConstant.TEXT_PLAN)]
            TextPlain = 4,

            [Description(SystemConstant.TEXT_HTML)]
            TextHtml = 5,


        }

        #endregion

        #region Direct Message Type

        public enum DirectMessageType
        {
            Text = 1,

            File = 3,

            Image = 2,

        }

        #endregion

        #region ResultCode

        public enum ResultCode
        {
            InsertSuccess = 400,

            UpdateSuccess = 401,

            ReadSuccess = 402,

            DeleteSuccess = 403,

            InvalidAction = -405,

        }

        #endregion

        #region Store Procedure Action Type

        public enum StoreProcedureActionType
        {
            // 'ADD_UPDATE'
            ADD_UPDATE = 1,

            //  'READ'
            READ = 2,

            // 'DELETE'
            DELETE = 3,

        }

        #endregion

    }
}