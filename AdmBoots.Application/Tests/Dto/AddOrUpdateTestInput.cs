using AdmBoots.Domain.Models;
using AutoMapper;

namespace AdmBoots.Application.Tests.Dto {

    /// <summary>
    ///
    /// </summary>
    [AutoMap(typeof(Test), ReverseMap = true)]
    public class AddOrUpdateTestInput {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
