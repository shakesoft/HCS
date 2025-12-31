using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HC.AbpUsers;

public abstract class AbpUsersAppServiceTests<TStartupModule> : HCApplicationTestBase<TStartupModule> where TStartupModule : IAbpModule
{
    private readonly IAbpUsersAppService _abpUsersAppService;
    private readonly IRepository<AbpUser, Guid> _abpUserRepository;

    public AbpUsersAppServiceTests()
    {
        _abpUsersAppService = GetRequiredService<IAbpUsersAppService>();
        _abpUserRepository = GetRequiredService<IRepository<AbpUser, Guid>>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Act
        var result = await _abpUsersAppService.GetListAsync(new GetAbpUsersInput());
        // Assert
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.AbpUser.Id == Guid.Parse("41a2de23-ddd2-4462-abfb-09e8258e0cca")).ShouldBe(true);
        result.Items.Any(x => x.AbpUser.Id == Guid.Parse("2b0c9752-ed28-4818-b7b3-c472cf9bc737")).ShouldBe(true);
    }

    [Fact]
    public async Task GetAsync()
    {
        // Act
        var result = await _abpUsersAppService.GetAsync(Guid.Parse("41a2de23-ddd2-4462-abfb-09e8258e0cca"));
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(Guid.Parse("41a2de23-ddd2-4462-abfb-09e8258e0cca"));
    }

    [Fact]
    public async Task CreateAsync()
    {
        // Arrange
        var input = new AbpUserCreateDto
        {
            UserName = "3853202d9769468386ca156fb9e701948347c2c1700543e99675c46416d369589dc0fa69b7fb4419ae410da9f4004aa6cd32d6ceeb804bb3a01d816c2a9aa5d280f1380222d3443aada03290b783acb89dbe4dba1bb7424e8f8b7b85b7facddc42c0724ed7204057a1346c56f85a34c7fd626f9d9e004a39b54b4cf2fc7bd401",
            NormalizedUserName = "be91d7d18e804c068a2024bdc55b3947dbe82279ae1d4fa586af38a0759cddd61054028b431c4f3eafb77637c3968edcc50985b38cfd42aa8ca66aa097990cba3f3d779e639a45ae94f4f204d402ac753b32ba5685e54c38bca3d5e9dfba89993e09cac52ec8424a8c020afeb4f931eac4ec2bd97c134cec9e8fb00ede8bb58d",
            Name = "f5d79c926a934bb598cc52232ac13273f44ab96746c54d63be25bcb4b1b0ab70",
            Surname = "ea6291e02f8648418987c08cff49b4b165033d1df7d54a9b8c5274dc79f12acc",
            Email = "b077854b09dc447c84525b6a218d6d0d83e1c529447e45deba5142df2afb0ea7077094168f684ba5a9ce37da354dc4ef65ae6df0dc134953b24112de4b1bca68eb8bd9517f6f4e92958fbea2b0760e9c6c1ebab5e3c44b32837e8e64d5d2214c48c60ad813e0466fb3f6ca687a9c9b5369762162770947ada7be9b6bf1a37635",
            NormalizedEmail = "4b46c1859b744f2bbfefa855adbdbc5691bc27aa928643afb8e1e6b65a488bada54a4c47fa994263a8297b553a6638a717148ea5201b485e86ad4d323ff392c595871acbba0945b4abce75cb723e716ff49535f2aa0045d2a45ab591dedf01e75322d9aee3db4573866ceb8a2af712e9b38973e2abd746a18c1b7dd350c308c0",
            EmailConfirmed = true,
            PasswordHash = "8300913b3a064f56a63d56ee7f76a826c35c4f0d32aa4b3d9fde7c369b5f06e9e6f8ca02cd47450a87c58568977c081d0d8688ea80524ee88bcd1915aadfda4478975fd7d7b44e7bb22c2bb7af1b142159ba2ab2dee641068fcf63d0298dd659078279febb5a421588473936baae2cde4560f10dda5741c8bcf4bce2e2eb0b8f",
            SecurityStamp = "e741ed1ed08249519175be584ee750689a7daf1e0f144adaae740d65a3de032c27f16145ff1542ce81daef6c1f9e3b79e419384b30d64fb69844023b313ea75ceef3c0e629e14f4ea2d580d9576bd7845da913bd715447538cbd2f627ff05a6b0bd2856c018448eaac319e3a4177b6b2384b45a111264106806932feb13a6dc8",
            IsExternal = true,
            PhoneNumber = "50d1d96386ca4a7f",
            PhoneNumberConfirmed = true,
            IsActive = true,
            TwoFactorEnabled = true,
            LockoutEnd = new DateTime(2000, 6, 2),
            LockoutEnabled = true,
            AccessFailedCount = 386326935,
            ShouldChangePasswordOnNextLogin = true,
            EntityVersion = 1041604239,
            LastPasswordChangeTime = new DateTime(2010, 10, 21)
        };
        // Act
        var serviceResult = await _abpUsersAppService.CreateAsync(input);
        // Assert
        var result = await _abpUserRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.UserName.ShouldBe("3853202d9769468386ca156fb9e701948347c2c1700543e99675c46416d369589dc0fa69b7fb4419ae410da9f4004aa6cd32d6ceeb804bb3a01d816c2a9aa5d280f1380222d3443aada03290b783acb89dbe4dba1bb7424e8f8b7b85b7facddc42c0724ed7204057a1346c56f85a34c7fd626f9d9e004a39b54b4cf2fc7bd401");
        result.NormalizedUserName.ShouldBe("be91d7d18e804c068a2024bdc55b3947dbe82279ae1d4fa586af38a0759cddd61054028b431c4f3eafb77637c3968edcc50985b38cfd42aa8ca66aa097990cba3f3d779e639a45ae94f4f204d402ac753b32ba5685e54c38bca3d5e9dfba89993e09cac52ec8424a8c020afeb4f931eac4ec2bd97c134cec9e8fb00ede8bb58d");
        result.Name.ShouldBe("f5d79c926a934bb598cc52232ac13273f44ab96746c54d63be25bcb4b1b0ab70");
        result.Surname.ShouldBe("ea6291e02f8648418987c08cff49b4b165033d1df7d54a9b8c5274dc79f12acc");
        result.Email.ShouldBe("b077854b09dc447c84525b6a218d6d0d83e1c529447e45deba5142df2afb0ea7077094168f684ba5a9ce37da354dc4ef65ae6df0dc134953b24112de4b1bca68eb8bd9517f6f4e92958fbea2b0760e9c6c1ebab5e3c44b32837e8e64d5d2214c48c60ad813e0466fb3f6ca687a9c9b5369762162770947ada7be9b6bf1a37635");
        result.NormalizedEmail.ShouldBe("4b46c1859b744f2bbfefa855adbdbc5691bc27aa928643afb8e1e6b65a488bada54a4c47fa994263a8297b553a6638a717148ea5201b485e86ad4d323ff392c595871acbba0945b4abce75cb723e716ff49535f2aa0045d2a45ab591dedf01e75322d9aee3db4573866ceb8a2af712e9b38973e2abd746a18c1b7dd350c308c0");
        result.EmailConfirmed.ShouldBe(true);
        result.PasswordHash.ShouldBe("8300913b3a064f56a63d56ee7f76a826c35c4f0d32aa4b3d9fde7c369b5f06e9e6f8ca02cd47450a87c58568977c081d0d8688ea80524ee88bcd1915aadfda4478975fd7d7b44e7bb22c2bb7af1b142159ba2ab2dee641068fcf63d0298dd659078279febb5a421588473936baae2cde4560f10dda5741c8bcf4bce2e2eb0b8f");
        result.SecurityStamp.ShouldBe("e741ed1ed08249519175be584ee750689a7daf1e0f144adaae740d65a3de032c27f16145ff1542ce81daef6c1f9e3b79e419384b30d64fb69844023b313ea75ceef3c0e629e14f4ea2d580d9576bd7845da913bd715447538cbd2f627ff05a6b0bd2856c018448eaac319e3a4177b6b2384b45a111264106806932feb13a6dc8");
        result.IsExternal.ShouldBe(true);
        result.PhoneNumber.ShouldBe("50d1d96386ca4a7f");
        result.PhoneNumberConfirmed.ShouldBe(true);
        result.IsActive.ShouldBe(true);
        result.TwoFactorEnabled.ShouldBe(true);
        result.LockoutEnd.ShouldBe(new DateTime(2000, 6, 2));
        result.LockoutEnabled.ShouldBe(true);
        result.AccessFailedCount.ShouldBe(386326935);
        result.ShouldChangePasswordOnNextLogin.ShouldBe(true);
        result.EntityVersion.ShouldBe(1041604239);
        result.LastPasswordChangeTime.ShouldBe(new DateTime(2010, 10, 21));
    }

    [Fact]
    public async Task UpdateAsync()
    {
        // Arrange
        var input = new AbpUserUpdateDto()
        {
            UserName = "1d6c76d4f6f949d7a11cdba377404dea24293faec64b453ab9369ae3c44d1298c189fd6c5e4b496b8b1bd6fef8d00bd771cd259053184e6bb0c8881c1dd2459499001ef98cc04ade8dd9e76b2bb8b64133e6e383c109421388014d337924ebd7fcd8e65d285f4592ac62e1763a77e90a3acc62b792734f83b5eae554bbb44157",
            NormalizedUserName = "501f88cf0fa5426e9bd7fc9e828773392dc05755da2e4dbba1b870224f6880e5ebf4e7e55b53465497350ce39edd8376fc27e998705f434e84e15730028fee2d7c53d7571b8a49d09e2d2132098817e8b02ca018277f4bceab055932dcf3937de53cdfef715f4be8b6591a7c7a5a45e51364f6b8f33c482fa30912c8801ddd2f",
            Name = "2ec6e94fe167402292aed58cf3a44aa45c66f15e2d714e00aa3c23a65e0f8479",
            Surname = "ff81f83874ec4cf4808fc16a50201373a988cbfb5346460fa53a45f4aa082da3",
            Email = "a62d76a841834ca4ba68c3e3db7c05a9777a3550018a4b7a94e1aedcdef39aca7224dfaf1a2a4070b3f4b34d62fdbb3580b360c40a59415780f598d74d6961243cb871847cd9453da9d7df17c36f4398c1db36e35baf428daec8a85aabd55f0813410d4aa39b424b9883f786175d27ec4e358be8ef7d48f2b6dc68474451d0c5",
            NormalizedEmail = "a7ce659e8a934b0996a2e7d107fcc5128b2d9584e18a461cb25fc9fa430b6799f837d687dcf44cf09d84fd2d99ced686a7cca3ae4f694be98c190aa580bfea589ae53ccb25ff4011b6af539bbebeb9712fb7c832366c476295517b34439d6edf9a642d5b23e54f7baab36dd3250fcdd7c4aa27be66e6470284549ecaab949d29",
            EmailConfirmed = true,
            PasswordHash = "7f3e24767dd14111aa30c44b4ddc6dd35497f5179e68496abcaf8ab2b8b166ff17e2f806eca14281a3a4866832a10cdc88bee2b4ae6b436d86dfa27a3a7737c113da0a4e32cb4746ae14a784f3f4a6481cb1148428274de7a07b0a6809bc424fc9b78cdb96574a079c559003d1628e3b94d89ef34f844664bc54fdc4b801c5c9",
            SecurityStamp = "50dfd96c68814082a783864f765d2dc1730bd39ac1a64cbba258c4cb88aeddfd16f316f06e7442c8a5b64d17a6f485228fa2bbe06f974a2a8b23fafc29e2475cf2ea671018974c89bb992b0cadf57bcb5c1242fcff464fe79fa34b8454a2c5ea8459c92fe1814db3bb0565bd42960955f799ba76b7764ec9a0b8ff6ff2da8b57",
            IsExternal = true,
            PhoneNumber = "15c319f964954df2",
            PhoneNumberConfirmed = true,
            IsActive = true,
            TwoFactorEnabled = true,
            LockoutEnd = new DateTime(2002, 8, 18),
            LockoutEnabled = true,
            AccessFailedCount = 1872515205,
            ShouldChangePasswordOnNextLogin = true,
            EntityVersion = 1335133274,
            LastPasswordChangeTime = new DateTime(2011, 6, 16)
        };
        // Act
        var serviceResult = await _abpUsersAppService.UpdateAsync(Guid.Parse("41a2de23-ddd2-4462-abfb-09e8258e0cca"), input);
        // Assert
        var result = await _abpUserRepository.FindAsync(c => c.Id == serviceResult.Id);
        result.ShouldNotBe(null);
        result.UserName.ShouldBe("1d6c76d4f6f949d7a11cdba377404dea24293faec64b453ab9369ae3c44d1298c189fd6c5e4b496b8b1bd6fef8d00bd771cd259053184e6bb0c8881c1dd2459499001ef98cc04ade8dd9e76b2bb8b64133e6e383c109421388014d337924ebd7fcd8e65d285f4592ac62e1763a77e90a3acc62b792734f83b5eae554bbb44157");
        result.NormalizedUserName.ShouldBe("501f88cf0fa5426e9bd7fc9e828773392dc05755da2e4dbba1b870224f6880e5ebf4e7e55b53465497350ce39edd8376fc27e998705f434e84e15730028fee2d7c53d7571b8a49d09e2d2132098817e8b02ca018277f4bceab055932dcf3937de53cdfef715f4be8b6591a7c7a5a45e51364f6b8f33c482fa30912c8801ddd2f");
        result.Name.ShouldBe("2ec6e94fe167402292aed58cf3a44aa45c66f15e2d714e00aa3c23a65e0f8479");
        result.Surname.ShouldBe("ff81f83874ec4cf4808fc16a50201373a988cbfb5346460fa53a45f4aa082da3");
        result.Email.ShouldBe("a62d76a841834ca4ba68c3e3db7c05a9777a3550018a4b7a94e1aedcdef39aca7224dfaf1a2a4070b3f4b34d62fdbb3580b360c40a59415780f598d74d6961243cb871847cd9453da9d7df17c36f4398c1db36e35baf428daec8a85aabd55f0813410d4aa39b424b9883f786175d27ec4e358be8ef7d48f2b6dc68474451d0c5");
        result.NormalizedEmail.ShouldBe("a7ce659e8a934b0996a2e7d107fcc5128b2d9584e18a461cb25fc9fa430b6799f837d687dcf44cf09d84fd2d99ced686a7cca3ae4f694be98c190aa580bfea589ae53ccb25ff4011b6af539bbebeb9712fb7c832366c476295517b34439d6edf9a642d5b23e54f7baab36dd3250fcdd7c4aa27be66e6470284549ecaab949d29");
        result.EmailConfirmed.ShouldBe(true);
        result.PasswordHash.ShouldBe("7f3e24767dd14111aa30c44b4ddc6dd35497f5179e68496abcaf8ab2b8b166ff17e2f806eca14281a3a4866832a10cdc88bee2b4ae6b436d86dfa27a3a7737c113da0a4e32cb4746ae14a784f3f4a6481cb1148428274de7a07b0a6809bc424fc9b78cdb96574a079c559003d1628e3b94d89ef34f844664bc54fdc4b801c5c9");
        result.SecurityStamp.ShouldBe("50dfd96c68814082a783864f765d2dc1730bd39ac1a64cbba258c4cb88aeddfd16f316f06e7442c8a5b64d17a6f485228fa2bbe06f974a2a8b23fafc29e2475cf2ea671018974c89bb992b0cadf57bcb5c1242fcff464fe79fa34b8454a2c5ea8459c92fe1814db3bb0565bd42960955f799ba76b7764ec9a0b8ff6ff2da8b57");
        result.IsExternal.ShouldBe(true);
        result.PhoneNumber.ShouldBe("15c319f964954df2");
        result.PhoneNumberConfirmed.ShouldBe(true);
        result.IsActive.ShouldBe(true);
        result.TwoFactorEnabled.ShouldBe(true);
        result.LockoutEnd.ShouldBe(new DateTime(2002, 8, 18));
        result.LockoutEnabled.ShouldBe(true);
        result.AccessFailedCount.ShouldBe(1872515205);
        result.ShouldChangePasswordOnNextLogin.ShouldBe(true);
        result.EntityVersion.ShouldBe(1335133274);
        result.LastPasswordChangeTime.ShouldBe(new DateTime(2011, 6, 16));
    }

    [Fact]
    public async Task DeleteAsync()
    {
        // Act
        await _abpUsersAppService.DeleteAsync(Guid.Parse("41a2de23-ddd2-4462-abfb-09e8258e0cca"));
        // Assert
        var result = await _abpUserRepository.FindAsync(c => c.Id == Guid.Parse("41a2de23-ddd2-4462-abfb-09e8258e0cca"));
        result.ShouldBeNull();
    }
}