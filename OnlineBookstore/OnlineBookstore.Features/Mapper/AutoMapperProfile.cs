using AutoMapper;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.AuthorFeatures;
using OnlineBookstore.Features.BookFeatures;
using OnlineBookstore.Features.CommentFeatures;
using OnlineBookstore.Features.GenreFeatures;
using OnlineBookstore.Features.OrderFeatures;
using OnlineBookstore.Features.OrderFeatures.OrderDetailFeatures;
using OnlineBookstore.Features.PublisherFeatures;
using OnlineBookstore.Features.UserFeatures;

namespace OnlineBookstore.Features.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateBookDto, Book>().ReverseMap();
        CreateMap<UpdateBookDto, Book>().ReverseMap();
        CreateMap<Book, GetBriefBookDto>()
            .ForMember(gb => gb.AuthorName, 
                opt =>
                opt.MapFrom(src => src.Author.FirstName + src.Author.LastName))
            .ReverseMap();
        CreateMap<Book, GetBookDto>()
            .ForMember(gb => gb.GenreIds, 
                opt =>
                    opt.MapFrom(src => src.Genres.Select(g => g.Id)))
            .ReverseMap();
        
        CreateMap<CreateGenreDto, Genre>().ReverseMap();
        CreateMap<UpdateGenreDto, Genre>().ReverseMap();
        CreateMap<GetGenreDto, Genre>().ReverseMap();
        CreateMap<GetBriefGenreDto, Genre>().ReverseMap();

        CreateMap<CreatePublisherDto, Publisher>().ReverseMap();
        CreateMap<UpdatePublisherDto, Publisher>().ReverseMap();
        CreateMap<GetPublisherDto, Publisher>().ReverseMap();
        CreateMap<GetBriefPublisherDto, Publisher>().ReverseMap();
        
        CreateMap<CreateAuthorDto, Author>().ReverseMap();
        CreateMap<UpdateAuthorDto, Author>().ReverseMap();
        CreateMap<GetAuthorDto, Author>().ReverseMap();
        
        CreateMap<CreateCommentDto, Comment>().ReverseMap();
        CreateMap<GetCommentDto, Comment>().ReverseMap();
        
        CreateMap<CreateOrderDto, Order>().ReverseMap();
        CreateMap<GetOrderDto, Order>()
            .ForMember(o => o.OrderStatus, 
                opt =>
                    opt.MapFrom(src => src.Status))
            .ReverseMap();

        CreateMap<AddOrderDetailDto, OrderDetail>().ReverseMap();
        CreateMap<GetOrderDetailDto, OrderDetail>().ReverseMap();
        
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.UserName,
                opt =>
                    opt.MapFrom(src => src.FirstName + src.LastName))
            .ReverseMap();
        CreateMap<GetUserDto, User>().ReverseMap();
    }
}