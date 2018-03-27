﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using Xunit;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;
using Tests.Configs;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestCreateMapGenerator
    {
        private DecodedEntityClass _bookInfo;
        private DecodedEntityClass _AuthorInfo;
        public TestCreateMapGenerator()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                _bookInfo = new DecodedEntityClass(typeof(Book), context);
                _AuthorInfo = new DecodedEntityClass(typeof(Author), context);
            }
        }

        [Fact]
        public void TestAuthorReadMappings()
        {
            //SETUP
            var maps = new MapperConfigurationExpression();

            //ATTEMPT
            var mapCreator = new CreateMapGenerator(typeof(AuthorNameDto), _bookInfo, null, null);
            mapCreator.Accessor.BuildReadMapping(maps);

            //VERIFY
            var config = new MapperConfiguration(maps);
            var entity = new Author {AuthorId = 1, Name = "Author", Email = "me@nospam.com"};
            var dto = config.CreateMapper().Map<AuthorNameDto>(entity);
            dto.Name.ShouldEqual("Author");
        }

        [Fact]
        public void TestBookReadMappingsWithConfig()
        {
            //SETUP
            var maps = new MapperConfigurationExpression();

            //ATTEMPT
            var mapCreator = new CreateMapGenerator(typeof(BookTitleAndCount), _bookInfo, null, new BookTitleAndCountConfig());
            mapCreator.Accessor.BuildReadMapping(maps);

            //VERIFY
            var config = new MapperConfiguration(maps);
            var entity = DddEfTestData.CreateFourBooks().Last();
            var dto = config.CreateMapper().Map<BookTitleAndCount>(entity);
            dto.Title.ShouldEqual("Quantum Networking");
            dto.ReviewsCount.ShouldEqual(2);
        }



    }
}