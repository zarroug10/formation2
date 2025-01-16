using System;
using API.Entities;

namespace API.interfaces;// usied to preveent nameing confilict vetween the files

public interface ITokenService // an interface for the  Token service
{

    string CreateToken(AppUser user); // a methode that takes ApUser as an object parms and return it as string
 
}
