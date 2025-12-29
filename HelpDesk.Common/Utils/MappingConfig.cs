using AutoMapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Common.Utils
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {


            CreateMap<ChatSessionDto, ChatSessionsCreateDto>().ReverseMap();
            CreateMap<ChatsTagsMappingDto, ChatsTagsMappingCreateDto>().ReverseMap();
            CreateMap<ChatTransfersDto, ChatTransfersCreateDto>().ReverseMap();
            CreateMap<ChatMessagesDto, ChatMessagesCreateDto>().ReverseMap();

            CreateMap<DirectMessageGetResultDTO, DirectMessageGetResponseDTO>().ReverseMap();
            CreateMap<ChatSessionsListDto, ChatSessionDto>().ReverseMap();
            CreateMap<PersonsDto, PersonsCreateDto>().ReverseMap();
            CreateMap<OrganizationsDto, OrganizationsCreateDto>().ReverseMap();
            CreateMap<OrganizationListDto, OrganizationDropdownResponseDto>().ReverseMap();
            CreateMap<PersonFilterRequestDto, PersonDropdownRequestDto>().ReverseMap();
            CreateMap<PersonsListDto, PersonDropdownResponseDto>().ReverseMap();
            CreateMap<PersonFilterRequestDto, PersonDropdownRequestDto>().ReverseMap();
            CreateMap<PersonsListDto, PersonDropdownResponseDto>().ReverseMap();
            CreateMap<DirectMessageGetResultDTO, DirectMessageGetResponseDTO>().ReverseMap();

            CreateMap<CategoryDto, CategoryCreateDto>().ReverseMap();
            CreateMap<CategoryDto, CategoryUpdateDto>().ReverseMap();
            CreateMap<ArticleFeedbackDto, ArticleFeedbackCreateDto>().ReverseMap();
            CreateMap<ArticleViewDto, ArticleViewCreateDto>().ReverseMap();
            CreateMap<ArticleCreateDto, ArticleRequestDto>().ReverseMap();
            CreateMap<ArticleCreateDto, ArticleUpdateDto>().ReverseMap();
            CreateMap<KbSearchAnalyticsDto, KbSearchAnalyticsCreateDto>().ReverseMap();
            CreateMap<ArticleTranslationDto, ArticleTranslationCreateDto>().ReverseMap();
            CreateMap<DirectMessageDeleteAttachmentResultDTO, DirectMessageDeleteAttachmentResponseDTO>().ReverseMap();

            CreateMap<DirectMessageRecentResultDTO, DirectMessagesRecentMessagesResponseDTO>().ReverseMap();
            CreateMap<DepartmentDto,DepartmentCreateDto>().ReverseMap();
            CreateMap<DepartmentDto,DepartmentUpdateDto>().ReverseMap();
            

             CreateMap<ChatShortCutResponseDTO, ChatShortCutResultDTO>().ReverseMap();

            // Reverse map with ignored field
            CreateMap<DirectMessageSendResponseDTO, DirectMessageSendResultDTO>()
                .ForMember(dest => dest.ResultCode, opt => opt.Ignore());

            CreateMap<DirectMessageSendResultDTO, DirectMessageSendResponseDTO>()
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dest => dest.Message, opt => opt.Ignore());

            // DirectMessageResultDTO => MarkDirectMessageAsReadResponseDTO
            CreateMap<DirectMessageResultDTO, DirectMessageMarkAsReadResponseDTO>()
                .ForMember(dest => dest.Message, opt => opt.Ignore());

            CreateMap<DirectMessageMarkAsReadResponseDTO, DirectMessageResultDTO>()
                .ForMember(dest => dest.ResultCode, opt => opt.Ignore());

            // DirectMessageResultDTO => UpdateDirectMessageResponseDTO
            CreateMap<DirectMessageResultDTO, DirectMessageUpdateResponseDTO>()
                .ForMember(dest => dest.Message, opt => opt.Ignore());

            CreateMap<DirectMessageUpdateResponseDTO, DirectMessageResultDTO>()
                .ForMember(dest => dest.ResultCode, opt => opt.Ignore());

            // DirectMessageResultDTO => DeleteDirectMessageResponseDTO
            CreateMap<DirectMessageResultDTO, DirectMessageDeleteResponseDTO>()
                .ForMember(dest => dest.Message, opt => opt.Ignore());


        }
    }
}