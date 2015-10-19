using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GToken.Models;
using Platform.Models;

namespace GToken.Web.MapperProfile
{
    public class EntityMappingProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            base.Configure();

            this.CreateMap<CreateReferralViewModel, ReferralCampaign>()
                .ForMember(d => d.is_override, s => s.MapFrom(src => string.Compare(src.is_override, "on", true) == 0 ? true : false))
                .ForMember(d => d.is_display_only, s => s.MapFrom(src => string.Compare(src.is_display_only, "on", true) == 0 ? true : false));

            this.CreateMap<ReferralCampaign, EditReferralViewModel>()
                .ForMember(x => x.games, src => src.Ignore())
                .ForMember(x => x.game_id, opt => opt.MapFrom(src => src.game_id))
                .ForMember(x => x.start_date, opt => opt.MapFrom(src => src.start_date))
                .ForMember(x => x.end_date, opt => opt.MapFrom(src => src.end_date))
                .ForMember(x => x.gtoken_per_download, opt => opt.MapFrom(src => src.gtoken_per_download))
                .ForMember(x => x.quantity, opt => opt.MapFrom(src => src.quantity))
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.status))
                .ForMember(d => d.is_override, s => s.MapFrom(src => src.is_override ? "on" : string.Empty))
                .ForMember(d => d.is_display_only, s => s.MapFrom(src => src.is_display_only ? "on" : string.Empty));
        }
    }
}