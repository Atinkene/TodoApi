using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace TodoApi.Models;

[CollectionName("Roles")]
public class AppRole : MongoIdentityRole<Guid>
{
}