using StudHunter.API.Infrastructure;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.ModelsDto;

public record ChatParticipantDto(
    Guid Id,
    string DisplayName,
    string Role
);

public record ChatDto(
    Guid Id,
    ChatParticipantDto Interlocutor,
    string LastMessage,
    DateTime? LastMessageAt,
    bool IsBlockedByMe,
    bool IsBlockedByInterlocutor
);

public record MessageDto(
    Guid Id,
    Guid? SenderId,
    string Content,
    DateTime SentAt,
    Guid? InvitationId,
    bool IsRead
);

public record SendMessageRequest(
    Guid ReceiverId, 
    string Content, 
    Guid? InvitationId = null
);

public static class ChatMapper
{
    public static ChatParticipantDto ToParticipantDto(User? user)
    {
        if (user == null || user.IsDeleted)
        {
            return new ChatParticipantDto(
                Guid.Empty,
                UserDefaultNames.DefaultDeletedAccountName,
                "None"
            );
        }

        string displayedName = user switch
        {
            Student s => $"{s.LastName} {s.FirstName}".Trim(),
            Employer e => e.Name,
            Administrator => UserRoles.Administrator,
            _ => user.Email ?? UserDefaultNames.DefaultUserName
        };

        string role = UserRoles.GetRole(user);

        return new ChatParticipantDto(
            user.Id,
            displayedName,
            role
        );
    }

    public static ChatDto ToDto(Chat chat, Guid currentUserId, bool isBlockedByMe = false, bool isBlockedByInterlocutor = false)
    {
        var interlocutor = chat.User1Id == currentUserId ? chat.User2 : chat.User1;

        return new ChatDto(
            chat.Id,
            ToParticipantDto(interlocutor),
            chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()?.Content ?? "Нет сообщений",
            chat.LastMessageAt,
            isBlockedByMe,
            isBlockedByInterlocutor
        );
    }

    public static MessageDto ToDto(Message message)
    {
        return new MessageDto(
            message.Id,
            message.SenderId,
            message.Content,
            message.SentAt,
            message.InvitationId,
            message.IsRead
        );
    }
}