namespace HelpDesk.Common.Enums;

public enum StatusCode
{

    // Success Codes (positive)
    Success = 0,
    ProjectUpdatedSuccessfully = 1,
    TwoFactorAuthVerified = 2,
    DeletedSuccessfully = 3,
    // RedirectToResgister = 201,
    ProjectMemberUpdatedSuccessfully = 202,
    ChatWidgetUpdatedSuccessfully = 203,
    PMDeletedSuccessfully = 204,
    ProjectDeletedSuccessfully = 205,
    ProjectEnabledSuccessfully = 206,
    ProjectDisabledSuccessfully = 207,

    // should be changed
    AlreadyExists = -101,
    DataNotFound = -102,
    NameAlreadyExists = -101,
    NotFound = -100,
    // should be changed

    // Client Errors (negative) 
    INVALID_COLUMN_NAME = -1,
    EmailAlreadyExists = -101,
    InvalidInvitationToken = -102,
    InvitationTokenExpired = -103,
    InvalidEmailVerificationToken = -104,
    EmailVerificationTokenExpired = -105,
    EmailNotRegistered = -106,
    InvalidPasswordResetToken = -107,
    PasswordResetTokenExpired = -108,
    UserNotFound = -109,
    InvitationNotFound = -110,




    // CanNotAdminOrSuperAdmin = -201,
    // AlreadyProjectMember = -202,
    AgentNotFound = -203,
    AllReadyInvitationAccepted = -204,
    AdminNotFound = -205,
    HasSomeoneReportsTo = -206,
    OpenTickets = -207,
    ActiveChatSession = -208,
    ProjectNameAlreadyExists = -209,
    ProjectNotFound = -210,
    AdminAlreadyExists = -211,
    CannotAddAgentAsAdmin = -212,
    AgentAlreadyExists = -213,
    CannotAddAdminAsAgent = -214,
    InvitationAlreadySent = -215,
    CannotAddSuperAdminAsAdmin = -216,
    CannotAddSuperAdminAsAgent = -217,



    InternalServerError = -500,
    InsertSuccessful = 1,
    UpdateSuccessful = 2,
    DeleteSuccessful = 3,
    WatcherAlreadyExists = -602,
    PersonBanned = -601,
    InvalidColumnCode = -603,

    ChatsTagsMappingAlreadyExists = -401,
    ChatSessionDataNotFound = -402,
    ChatMessageDataNotFound = -403,
    ChatsTagsMappingDataNotFound = -404,
    ChatTransferDataNotFound = -405,
    PersonDataNotFound = -406,
    OrganizationDataNotFound = -407,
    PersonHasActiveTicket = -408,
    PersonHasActiveChat = -409,
    ChatMessageDeleteWindowExpired = -410,
    ChatMessageEditWindowExpired = -411,

    
    

}


