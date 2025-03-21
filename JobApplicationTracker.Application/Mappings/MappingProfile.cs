using System;
using AutoMapper;
using JobApplicationTracker.Application.DTOs;
using JobApplicationTracker.Domain.Entities;

namespace JobApplicationTracker.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between domain entities and DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // JobApplication mappings
            CreateMap<JobApplication, JobApplicationDto>();

            CreateMap<CreateJobApplicationDto, JobApplication>()
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => (DateTime?)null));

            CreateMap<UpdateJobApplicationDto, JobApplication>()
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore());

            CreateMap<UpdateApplicationStatusDto, JobApplication>()
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CompanyName,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Position,
                    opt => opt.Ignore())
                .ForMember(dest => dest.DateApplied,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ContactPerson,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ContactEmail,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Notes,
                    opt => opt.Ignore())
                .ForMember(dest => dest.JobUrl,
                    opt => opt.Ignore())
                .ForMember(dest => dest.SalaryRange,
                    opt => opt.Ignore());
        }
    }
}