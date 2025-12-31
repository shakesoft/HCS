using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using HC.AbpUsers;
using HC.EntityFrameworkCore;
using Xunit;

namespace HC.EntityFrameworkCore.Domains.AbpUsers;

public class AbpUserRepositoryTests : HCEntityFrameworkCoreTestBase
{
    private readonly IAbpUserRepository _abpUserRepository;

    public AbpUserRepositoryTests()
    {
        _abpUserRepository = GetRequiredService<IAbpUserRepository>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _abpUserRepository.GetListAsync(userName: "5b2cdd4370ce44e9b7961d2dc95e7a40a654bddfac1a423baebedfb67d106369c375febf9fff49b6984f50999fc1a70df807d4a54ba94a56a02fdd6317cbdb1b8c045ba6a4f040d5ae241787c9843cf26e4e692024894f0783aa0af57a407cba852504b73d1148c0bc04a852b4fbe7cdbb5c8a21866547988e5d995e18bddb9b", normalizedUserName: "b3be14684dd24083af9ed9ffccf854a9625fa7bc17004dd7a54a700f7dd0b9ecbd4fb44dcee545139b3e1dc332f937d415df45e15fff495f843cb603c9619fb89246fddfc2d8467f83f64b9b684ebd7d0a6421ae5a8046158159dff567c8a78633a167c3451e415a80fcae73f6bf8f6ddb35c99e85cc45a0b63a94f6d9b7c517", name: "f55ee0ff5b894d2e9d666b1e620faa1cc1cd24aaa5614980bf489ce7dff9d5a0", surname: "1be6aa807c5c42e88e6fa1f2f109481b947eac3cbf284bec803049a625c61b16", email: "14230ddbc2ba48b79e9ca4d2e27c9a6f7ff4ad1edfaf4dbfbf0b89ea87f8068be7e3c80496074e46b1d3f293985942661aa88e6f87284291af46d8b7b079ec8142ccab70649b4694b264ae781920293cc8f204d836f24f2da9f5951dcbc113081964840f19394d5ab526497f7a01d365b3d273b9b613496294f85bf594136fa4", normalizedEmail: "b1c69e3818ca49d0ad26325715bf73c454f084d5f30345febb1a147b6e06bb94990958cf32e24ebbbdf52dc865499bc0e6974251ca694a62aa8e1dfe6b0d249221e64395edfb47bf9094457e286d5c786b3fcdd38cdb4ac2a69e209d0795b0ad9bd892fd92234b64b11c793ce0a698c94a4381f9cc504e6cb947586666d93178", emailConfirmed: true, passwordHash: "8c4fd102b708475eb6be79f0ef424ab7cb4a75b33bd34fd8bf4acefdb6066c874a64b05c3b444abca7e07b1afcd093201813eb0f95374cd9b899557d9fcd762262ac564a56414274994db5d30536265f26af3e5ad0a24bb2b1ae0ee01005423bea87e8094dbd4f67b912fd4f7ba2ca1b21dc21e0e23b4094bdbecf63ade8e7d3", securityStamp: "75c567382d2e4311979feeecd4d3c04d5fc7a958e9ca4599b3b28d17065a1d1206015b2e54ba48faad4efe233b68c953f718d436f1ef448a8850b04c4ebb0b9024641ec1b2d0430d86042707a4a8a48aaec88b23d9d240daa96c3df9c9fb93b420d5e2c939754f52b9e7a506dfaec52414e2cac5e8664e3e8ac380d4cef0de47", isExternal: true, phoneNumber: "73f24c2650a940ab", phoneNumberConfirmed: true, isActive: true, twoFactorEnabled: true, lockoutEnabled: true, shouldChangePasswordOnNextLogin: true);
            // Assert
            result.Count.ShouldBe(1);
            result.FirstOrDefault().ShouldNotBe(null);
            result.First().Id.ShouldBe(Guid.Parse("41a2de23-ddd2-4462-abfb-09e8258e0cca"));
        });
    }

    [Fact]
    public async Task GetCountAsync()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () => {
            // Act
            var result = await _abpUserRepository.GetCountAsync(userName: "166349b135aa43658d79380fad69fa1eafbd0a4180e24571b77da987ebef384ef213ba1fe3b94095bfb0d04a84797627143777962d4b45529906644bf3016e43e397e3e00d6d4418afb6e00b46986f6bf7904e870ec44de1adb4446c78f8d65604c635e5dcbd4a57b9dd43b3de6bc0b40aee208979ce4b3587ecfcebcb49a2a2", normalizedUserName: "00c5d19938664050a772bc292b91dea10a4c6159311f4412b2cc701de69d14c29c12efe43fb04e6d851e607e1c36767b51812e88c8824defb934c063a65127b69ceb364b4d1644ebb6c5586b8021446c1b581501f6d0445cb833cd84d0a785c154eb2068ada44779a6a687fe9208f22aa9dd8733f44240578a5cd96459b5b510", name: "a29069cef2b7458faba8eab60adf3d94e7040551289a45cb86bf6b4b90d6953c", surname: "9356dd8a124e405db7b55fbb9693ddc8722863df50dd41eba767a279f0783c3b", email: "9feef98a6797472c93cc2ed62fefeeb63f4a0435e193420f9812277ec05c4ee780e35cf86ab6484e82b40f252b8a293ad782964513f04ec3839b318ce908e61438687e7ed29f4f5498f4f301513c2df437a3b7e316894765ac276cf0b602c3567d8c2662097742b9a69465c9c5fe3c636784e1c539064ab588a9d7b03d1cf5d9", normalizedEmail: "223c93e431844d6eacc3c8755d84967dbacf73ce8e9f47eea8a5fea0b051dccf2c1150225d1f450187b0e4fbe9f82cf37ad96cccd6c84826982eaf650ad781ad982c82cc05a64b848d5a3c592cd77c6a3fa4cab9dd514d79b729d9b3476a6d6a1d30d5d0b1fc4333985fb023fe90c69ca7e0457a035e475fb996481b5e18d731", emailConfirmed: true, passwordHash: "d7caa6ebd0ac4784acb73edca1861ac14e4398ea55b24495b83b92d4e7143a22ccbeb946a42b4e96b0e8a22b1010a49b4fe1e030838a4345a2e50237c4ba874d6d3a6a7788384f28acda6ea4946f3f5c67b8a9ca32c64399989a119673a4f91aa14d7fde59f840288c560dd3ae71bd10f210c3d94f1f4ac0868c521319fff199", securityStamp: "5543dd9610c1446c91ca9f8861a1cb171ca1351eb4374c6eb6df10f7863a1c8635c6fb2998d64e849d759f1fcc48c753173ae0008f2b4ee595e0999a6ca3406a756d822afb3248bca81b34c351ead325de5504063a8f470c8dffc93a75110700e79b1bf259c64ab6b9b3ce2d7c4a1a72414400c981b548a9bf3f9ed2faaddc3a", isExternal: true, phoneNumber: "423ef17da0f54c77", phoneNumberConfirmed: true, isActive: true, twoFactorEnabled: true, lockoutEnabled: true, shouldChangePasswordOnNextLogin: true);
            // Assert
            result.ShouldBe(1);
        });
    }
}