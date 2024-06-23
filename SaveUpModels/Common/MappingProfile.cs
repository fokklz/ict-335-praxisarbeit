using AutoMapper;
using SaveUpModels.Common.Options;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;

namespace SaveUpModels.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<Source, Destination>();
            CreateAdminMap<UpdateItemRequest, Item>();
            CreateAdminMap<UpdateUserRequest, User>();

            CreateAdminMap<CreateItemRequest, Item>();
            CreateAdminMap<CreateUserRequest, User>();

            CreateAdminMap<RegisterRequest, User>();

            CreateAdminMap<User, UserResponse>(true);
            CreateAdminMap<Item, ItemResponse>(true);
        }

        public void CreateAdminMap<TSrc, TDest>(bool source = false)
        {
            CreateMap<TSrc, TDest>().ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember, context) =>
            {
                if (srcMember == null)
                {
                    return false;
                }

                DTOParseOptions dtoOptions;

                var targetType = source ? src?.GetType() : dest?.GetType();
                var targetProperty = targetType?.GetProperty(opts.DestinationMember.Name);

                // parse automapper context options to our internal options class
                if (context.TryGetItems(out var items))
                {
                    var isOwner = (bool)items.GetValueOrDefault("IsOwner", false);
                    var isAdmin = (bool)items.GetValueOrDefault("IsAdmin", false);

                    dtoOptions = new DTOParseOptions
                    {
                        IsAdmin = isAdmin,
                        IsOwner = isOwner
                    };
                }
                else
                {
                    dtoOptions = new DTOParseOptions();
                }

                return ModelUtils.IsAllowed(targetProperty, dtoOptions);
            }));

        }
    }
}
