using Microsoft.AspNetCore.Http;

namespace IdentityProject.Utilities.File;

public interface IFileService
{
    string Upload(IFormFile file, string folder);
    void Delete(string folder, string fileName);
    bool IsImage(string contentType);
    bool IsTrueSize(long length, long maxSize = 500);

}
