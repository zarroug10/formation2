using System;

using API.interfaces;

namespace API.Data;

//Injecting a Scoped Services into the Unit Of Work Scoped service
public class UnitOfWork(DataContext context, IUserRepository userRepository
, IMessageRepository messageRepository, ILikesRepository likesRepository,IPhotoRepository photoRepository) : IUnitOfWork
{

    //encapsulation
    public IUserRepository UserRepository => userRepository;
    public IMessageRepository MessageRepository => messageRepository;
    public ILikesRepository LikesRepository =>  likesRepository;
    public IPhotoRepository PhotoRepository =>  photoRepository;



    //abstraction 
    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0 ;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
