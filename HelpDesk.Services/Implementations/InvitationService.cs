using Microsoft.Extensions.Localization;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Helpers;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Provides functionality to manage project invitations, including retrieving invitation details,
/// accepting, and rejecting invitations.
/// </summary>
public class InvitationService(IInvitationRepository invitationRepository,
    IStringLocalizer<Messages> localizer) : IInvitationService
{
    private readonly IInvitationRepository _invitationRepository = invitationRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    /// <summary>
    /// Retrieves invitation details using the specified token.
    /// </summary>
    /// <param name="token">The unique invitation token.</param>
    /// <returns>An <see cref="InvitationResultDTO"/> containing invitation information.</returns>
    /// <exception cref="NotFoundException">Thrown when no invitation is found for the given token.</exception>
    public async Task<InvitationResultDTO?> GetInvitationDetailsByTokenAsync(Guid token)
    {
        InvitationResultDTO? invitation = await _invitationRepository.GetInvitationDetailsByTokenAsync(token)
            ?? throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["Invitation"]]);

        return invitation;
    }

    /// <summary>
    /// Accepts an invitation based on the provided request data.
    /// </summary>
    /// <param name="dto">The request containing invitation token and user details.</param>
    /// <exception cref="DataAlreadyExistsException">Thrown when the email already exists.</exception>
    /// <exception cref="BadRequestException">Thrown for expired, invalid, or already accepted tokens.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task AcceptInvitationAsync(AcceptOrRejectInvitationRequestDTO dto)
    {
        int result = await _invitationRepository.AcceptInvitationAsync(dto);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.EmailAlreadyExists => new DataAlreadyExistsException(_localizer["ENTITY_EXISTS", _localizer["Email"]]),
                StatusCode.InvitationTokenExpired => new BadRequestException(_localizer["INVITATION_TOKEN_EXPIRED"]),
                StatusCode.InvalidInvitationToken => new BadRequestException(_localizer["INVALID_INVITATION_TOKEN"]),
                StatusCode.AllReadyInvitationAccepted => new BadRequestException(_localizer["INVITATION_TOKEN_ALREADY_ACCEPTED"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Rejects an invitation based on the provided request data.
    /// </summary>
    /// <param name="dto">The request containing invitation token and user details.</param>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected server error occurs.</exception>
    public async Task RejectInvitationAsync(AcceptOrRejectInvitationRequestDTO dto)
    {
        int result = await _invitationRepository.RejectInvitationAsync(dto);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result) && statusCode == StatusCode.InternalServerError)
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
    }

}
