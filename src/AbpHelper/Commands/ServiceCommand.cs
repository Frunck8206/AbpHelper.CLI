﻿using EasyAbp.AbpHelper.Extensions;
using EasyAbp.AbpHelper.Steps.Abp;
using EasyAbp.AbpHelper.Steps.Common;
using Elsa.Activities;
using Elsa.Expressions;
using Elsa.Scripting.JavaScript;
using Elsa.Services;
using System;
using System.Threading.Tasks;
using Humanizer;

namespace EasyAbp.AbpHelper.Commands
{
    public class ServiceCommand : CommandWithOption<ServiceCommandOption>
    {
        public ServiceCommand(IServiceProvider serviceProvider) 
            : base(serviceProvider, "service", "Generate service interface and class files according to the specified name")
        {
        }

        protected override Task RunCommand(ServiceCommandOption option)
        {
            if (option.Folder.IsNullOrEmpty())
            {
                option.Folder = option.Name.Pluralize();
            }
            option.Folder = option.Folder.NormalizePath();
            return base.RunCommand(option);
        }

        protected override IActivityBuilder ConfigureBuild(ServiceCommandOption option, IActivityBuilder activityBuilder)
        {
            return base.ConfigureBuild(option, activityBuilder)
                .Then<SetVariable>(
                    step =>
                    {
                        step.VariableName = "TemplateDirectory";
                        step.ValueExpression = new LiteralExpression<string>("/Templates/Service");
                    })
                .Then<ProjectInfoProviderStep>()
                .Then<SetModelVariableStep>()
                .Then<GroupGenerationStep>(
                    step =>
                    {
                        step.GroupName = "Service";
                        step.TargetDirectory = new JavaScriptExpression<string>("AspNetCoreDir");
                    });
        }
    }
}