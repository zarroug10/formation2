using System;

namespace API.DTO;

public class CreateMessageDTO
{
    public required string RecipientUsername { get; set; }
    public required string Content { get; set; }
}
