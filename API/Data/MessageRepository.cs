using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class MessageRepository : IMessageRepository {
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;

    public MessageRepository(IMapper mapper,DataContext dataContext) {
        _mapper = mapper;
        _dataContext = dataContext;
    }

    public void AddMessage(Message message) => _dataContext.Messages.Add(message);

    public void DeleteMessage(Message message) => _dataContext.Messages.Remove(message);

    public async Task<Message?> GetMessage(int id) => await _dataContext.Messages.FindAsync(id);
#nullable disable
    public async Task<IEnumerable<MessageDto>> GetMessageThread(string SenderUsername , string recipientUserName)
   {
    var messages = await _dataContext.Messages
                    .Include(ms => ms.Sender).ThenInclude(user => user.Photos)
                    .Include(ms => ms.Recipient).ThenInclude(user => user.Photos)
                    .Where(ms =>
                        (ms.RecipientUsername == SenderUsername 
                        && ms.IsRecipientDeleted == false 
                        && ms.SenderUsername == recipientUserName) 
                        ||
                        (ms.RecipientUsername == recipientUserName
                         && ms.IsSenderDeleted == false 
                         && ms.SenderUsername == SenderUsername)
                    )
                    // .OrderByDescending(ms => ms.DateSent)
                    .OrderBy(ms => ms.DateSent)
                    .ToListAsync();

    var unreadMessages = messages
                    .Where(ms => ms.DateRead == null && ms.RecipientUsername == SenderUsername)
                    .ToList();

    if (unreadMessages.Any())
    {
        foreach (var ms in unreadMessages)
            ms.DateRead = DateTime.UtcNow;
        await _dataContext.SaveChangesAsync();
    }
    return _mapper.Map<IEnumerable<MessageDto>>(messages);
  }
   public async Task<PageList<MessageDto>> GetUserMessages(MessageParams messageParams){
    var query = _dataContext.Messages.OrderByDescending(ms => ms.DateSent).AsQueryable();
    query = messageParams.Label switch {
      "Inbox" => query.Where(ms => ms.RecipientUsername == messageParams.Username && ms.IsRecipientDeleted == false),
      "Sent" => query.Where(ms => ms.SenderUsername == messageParams.Username && ms.IsSenderDeleted == false    ),
      _ => query.Where(ms => ms.RecipientUsername == messageParams.Username && ms.IsRecipientDeleted == false && ms.DateRead == null)
    };
    var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
    return await PageList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
  }
    public async Task<bool> SaveAllAsync() => await _dataContext.SaveChangesAsync() > 0;
}