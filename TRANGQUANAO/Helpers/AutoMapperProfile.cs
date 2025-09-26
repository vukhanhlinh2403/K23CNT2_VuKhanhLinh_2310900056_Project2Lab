using TRANGQUANAO.Models;
using TRANGQUANAO.ViewModels;
using AutoMapper;
using TRANGQUANAO.Models;
using TRANGQUANAO.ViewModels;

namespace WEBQUANAO.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, KhachHang>();
            //.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen))
            //.ReverseMap();
        }
    }
}
