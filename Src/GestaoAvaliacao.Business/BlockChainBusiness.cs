﻿using System;
using System.Linq;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Util;

namespace GestaoAvaliacao.Business
{
    public class BlockChainBusiness : IBlockChainBusiness
    {
        private readonly IBlockChainRepository blockChainRepository;
        private readonly ITestRepository testRepository;

        public BlockChainBusiness(IBlockChainRepository blockChainRepository, ITestRepository testRepository)
        {
            this.blockChainRepository = blockChainRepository;
            this.testRepository = testRepository;
        }

        #region Custom

        private static Validate Validate(BlockChain entity, Validate valid, Guid usuId, EnumSYS_Visao vision)
        {
            valid.Message = null;

            var duplicateOrder = entity.BlockChainItems?.GroupBy(x => x.Order)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key).ToList();

            if (duplicateOrder != null && duplicateOrder.Any())
                valid.Message = "Existe mais de um item com a mesma ordem.";

            if (!(entity.Test.UsuId == usuId || (vision == EnumSYS_Visao.Administracao && entity.Test.TestType.Global)))
                valid.Message = "Apenas o proprietário da prova pode alterá-la";

            if (!string.IsNullOrEmpty(valid.Message))
            {
                const string br = "<br/>";

                valid.Message = valid.Message.TrimStart(br.ToCharArray());

                valid.IsValid = false;

                if (valid.Code <= 0)
                    valid.Code = 400;

                valid.Type = ValidateType.alert.ToString();
            }
            else
                valid.IsValid = true;

            return valid;
        }

        #endregion


        public BlockChain Save(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision)
        {
            if (blockChain.Test_Id != null)
            {
                var test = testRepository.GetObjectWithTestType(Convert.ToInt64(blockChain.Test_Id));

                blockChain.Test = test;
                blockChain.Test_Id = test.Id;
            }

            blockChain.Validate = Validate(blockChain, blockChain.Validate, usuId, vision);

            if (!blockChain.Validate.IsValid) 
                return blockChain;

            blockChain = blockChainRepository.Save(blockChain);
            blockChain.Validate.Type = ValidateType.Save.ToString();

            return blockChain;
        }

        public BlockChain Update(BlockChain blockChain, Guid usuId, EnumSYS_Visao vision)
        {
            if (blockChain.Test_Id != null)
            {
                var test = testRepository.GetObjectWithTestType(Convert.ToInt64(blockChain.Test_Id));
                blockChain.Test = test;
                blockChain.Test_Id = test.Id;
            }

            blockChain.Validate = Validate(blockChain, blockChain.Validate, usuId, vision);

            if (!blockChain.Validate.IsValid) 
                return blockChain;

            blockChainRepository.Update(blockChain);
            blockChain.Validate.Type = ValidateType.Update.ToString();

            return blockChain;
        }
    }
}