namespace BlazorECommerce.Server.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly DataContext _dataContext;

    public CategoryService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    public async Task<ServiceResponse<List<Category>>> getCategories()
    {
        var categories = await _dataContext.Categories.ToListAsync();
        var response = new ServiceResponse<List<Category>>
        {
            Data = categories
        };
        return response;
    }
}
