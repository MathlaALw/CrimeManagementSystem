using Crime_Management_System.DTOs;
using Crime_Management_System.Models;
using AutoMapper;

namespace Crime_Management_System.Mapping
{
    public class CrimeMappingProfile : Profile
    {
        public CrimeMappingProfile()
        {
            // ---------- Users ----------
            // Create user: we don't hash here; service should set PasswordHash.
            CreateMap<CreateUserDto, User>()
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());

            // Update user: only map non-null fields
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

            //// ---------- Cases ----------
            //CreateMap<CreateCaseDto, Case>()
            //    .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            //    .ForMember(d => d.CreatedAt, o => o.Ignore());

            //CreateMap<UpdateCaseDto, Case>()
            //    .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

            // Case -> CaseListItemDto (uses CreatedByUser and truncates Description like service)
            CreateMap<Case, CaseListItemDto>()
                .ForCtorParam("Id", o => o.MapFrom(s => s.Id))
                .ForCtorParam("CaseNumber", o => o.MapFrom(s => s.CaseNumber))
                .ForCtorParam("Name", o => o.MapFrom(s => s.Name))
                .ForCtorParam("Description", o => o.MapFrom(s => s.Description))
                .ForCtorParam("AreaCity", o => o.MapFrom(s => s.AreaCity))
                .ForCtorParam("CaseType", o => o.MapFrom(s => s.CaseType))
                .ForCtorParam("CreatedBy", o => o.MapFrom(s => s.CreatedByUser.FullName))
                .ForCtorParam("CreatedAt", o => o.MapFrom(s => s.CreatedAt))
                .ForCtorParam("AuthorizationLevel", o => o.MapFrom(s => s.AuthorizationLevel));

            CreateMap<Case, CaseDetailsDto>()
                 .ForCtorParam("Id", opt => opt.MapFrom(s => s.Id))
                 .ForCtorParam("CaseNumber", opt => opt.MapFrom(s => s.CaseNumber))
                 .ForCtorParam("Name", opt => opt.MapFrom(s => s.Name))
                 .ForCtorParam("Description", opt => opt.MapFrom(s => s.Description))
                 .ForCtorParam("AreaCity", opt => opt.MapFrom(s => s.AreaCity))
                 .ForCtorParam("CaseType", opt => opt.MapFrom(s => s.CaseType))
                 .ForCtorParam("Status", opt => opt.MapFrom(s => s.Status))
                 .ForCtorParam("AuthorizationLevel", opt => opt.MapFrom(s => s.AuthorizationLevel))
                 .ForCtorParam("CreatedBy", opt => opt.MapFrom(s => s.CreatedByUser.FullName))
                 .ForCtorParam("CreatedAt", opt => opt.MapFrom(s => s.CreatedAt))
                 .ForCtorParam("ReportedBy", opt => opt.MapFrom(s => s.CreatedByUserId)) // <- adjust to your model
                 .ForCtorParam("Assignees", opt => opt.MapFrom(s => s.CaseAssignees.Count))
                 .ForCtorParam("Evidences", opt => opt.MapFrom(s => s.Evidences.Count))
                 .ForCtorParam("Suspects", opt => opt.MapFrom(s => s.CaseParticipants.Count(p => p.Role == ParticipantRole.Suspect)))
                 .ForCtorParam("Victims", opt => opt.MapFrom(s => s.CaseParticipants.Count(p => p.Role == ParticipantRole.Victim)))
                 .ForCtorParam("Witnesses", opt => opt.MapFrom(s => s.CaseParticipants.Count(p => p.Role == ParticipantRole.Witness)));


                        // ---------- Reports ----------
            CreateMap<SubmitCrimeReportDto, CrimeReport>()
                .ForMember(d => d.ReportDateTime, o => o.Ignore()) // defaulted in entity
                .ForMember(d => d.Status, o => o.Ignore()); // defaulted to "pending" in entity

            // ---------- Participants ----------
            CreateMap<AddParticipantDto, Participant>();

            // Link participant to case: service fills CaseId and AddedByUserId
            CreateMap<AddParticipantToCaseDto, CaseParticipant>()
                .ForMember(d => d.CaseId, o => o.Ignore())
                .ForMember(d => d.AddedByUserId, o => o.Ignore())
                .ForMember(d => d.AddedAt, o => o.Ignore());

            // ---------- Evidence ----------
            // Text evidence
            CreateMap<CreateTextEvidenceDto, Evidence>()
                .ForMember(d => d.Type, o => o.MapFrom(_ => EvidenceType.Text))
                .ForMember(d => d.AddedByUserId, o => o.Ignore())
                .ForMember(d => d.FileUrl, o => o.Ignore())
                .ForMember(d => d.MimeType, o => o.Ignore())
                .ForMember(d => d.SizeBytes, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());

            // Image evidence (file handling stays in service)
            CreateMap<CreateImageEvidenceDto, Evidence>()
                .ForMember(d => d.Type, o => o.MapFrom(_ => EvidenceType.Image))
                .ForMember(d => d.TextContent, o => o.Ignore())
                .ForMember(d => d.AddedByUserId, o => o.Ignore())
                .ForMember(d => d.FileUrl, o => o.Ignore())
                .ForMember(d => d.MimeType, o => o.Ignore())
                .ForMember(d => d.SizeBytes, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());

            // Update evidence (only non-null fields)
            CreateMap<UpdateEvidenceDto, Evidence>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
