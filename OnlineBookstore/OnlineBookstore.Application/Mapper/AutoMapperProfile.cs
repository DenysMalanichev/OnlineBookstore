using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using OnlineBookstore.Application.Author.Create;
using OnlineBookstore.Application.Author.Update;
using OnlineBookstore.Application.Books.Create;
using OnlineBookstore.Application.Books.Dtos;
using OnlineBookstore.Application.Books.Update;
using OnlineBookstore.Application.Comments.Dtos;
using OnlineBookstore.Application.Genres.Create;
using OnlineBookstore.Application.Genres.Dtos;
using OnlineBookstore.Application.Genres.Update;
using OnlineBookstore.Application.OrderDetails.Create;
using OnlineBookstore.Application.OrderDetails.Dtos;
using OnlineBookstore.Application.Orders.CloseUsersOrder;
using OnlineBookstore.Application.Orders.Create;
using OnlineBookstore.Application.Orders.Dtos;
using OnlineBookstore.Application.Publishers.Create;
using OnlineBookstore.Application.Publishers.Update;
using OnlineBookstore.Domain.Entities;
using OnlineBookstore.Features.AuthorFeatures;
using OnlineBookstore.Features.PublisherFeatures;
using OnlineBookstore.Features.UserFeatures;

namespace OnlineBookstore.Application.Mapper;

[ExcludeFromCodeCoverage]
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateBookCommand, Book>().ReverseMap();
        CreateMap<UpdateBookCommand, Book>().ReverseMap();
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
        
        CreateMap<CreateGenreCommand, Genre>().ReverseMap();
        CreateMap<UpdateGenreCommand, Genre>().ReverseMap();
        CreateMap<GetGenreDto, Genre>().ReverseMap();
        CreateMap<GetBriefGenreDto, Genre>().ReverseMap();

        CreateMap<CreatePublisherCommand, Publisher>().ReverseMap();
        CreateMap<UpdatePublisherCommand, Publisher>().ReverseMap();
        CreateMap<GetPublisherDto, Publisher>().ReverseMap();
        CreateMap<GetBriefPublisherDto, Publisher>().ReverseMap();
        
        CreateMap<CreateAuthorCommand, Domain.Entities.Author>().ReverseMap();
        CreateMap<UpdateAuthorCommand, Domain.Entities.Author>().ReverseMap();
        CreateMap<GetAuthorDto, Domain.Entities.Author>().ReverseMap();
        
        CreateMap<CreateCommentDto, Comment>().ReverseMap();
        CreateMap<GetCommentDto, Comment>().ReverseMap();

        CreateMap<CloseOrderData, CloseUsersOrderCommand>().ReverseMap();
        CreateMap<CreateOrderCommand, Order>().ReverseMap();
        CreateMap<GetOrderDto, Order>()
            .ForMember(o => o.OrderStatus, 
                opt =>
                    opt.MapFrom(src => src.Status))
            .ReverseMap();

        CreateMap<CreateOrderDetailCommand, OrderDetail>().ReverseMap();
        CreateMap<GetOrderDetailDto, OrderDetail>().ReverseMap();
        
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.UserName,
                opt =>
                    opt.MapFrom(src => src.FirstName + src.LastName))
            .ReverseMap();
        CreateMap<GetUserDto, User>().ReverseMap();
    }
}