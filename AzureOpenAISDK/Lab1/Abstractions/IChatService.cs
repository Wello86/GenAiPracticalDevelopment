using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab1.Abstractions;

public interface IChatService
{
    Task<string?> SendMessageAsync(string message);
}