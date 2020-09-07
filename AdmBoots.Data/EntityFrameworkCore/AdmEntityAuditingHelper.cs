using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Infrastructure.Framework.Web;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Data.EntityFrameworkCore {

    public class AdmEntityAuditingHelper {

        public static void SetCreationAuditProperties(object entityAsObj) {
            var entityWithCreationTime = entityAsObj as CreationEntity<int>;
            if (entityWithCreationTime == null) {
                //Object does not implement CreationEntity
                return;
            }
            var admSession = AdmBootsApp.ServiceProvider.GetRequiredService<IAdmSession>();
            var userId = admSession.UserId;
            var userName = admSession.UserName;
            if (entityWithCreationTime.CreateTime.GetValueOrDefault() == default(DateTime)) {
                entityWithCreationTime.CreateTime = DateTime.Now;
            }

            if (!userId.HasValue || userId == 0) {
                //Unknown user
                return;
            }

            if (entityWithCreationTime.CreatorId != 0) {
                return;
            }

            //Finally, set CreatorUserId!
            entityWithCreationTime.CreatorId = (int)userId;
            entityWithCreationTime.CreatorName = userName;
        }

        public static void SetModificationAuditProperties(object entityAsObj) {
            var entityWithAudit = entityAsObj as AuditEntity;
            if (entityWithAudit == null) {
                return;
            }
            var admSession = AdmBootsApp.ServiceProvider.GetRequiredService<IAdmSession>();
            var userId = admSession.UserId;
            var userName = admSession.UserName;
            entityWithAudit.ModifyTime = DateTime.Now;

            if (!userId.HasValue || userId == 0) {
                //Unknown user
                entityWithAudit.ModifierId = null;
                entityWithAudit.ModifierName = null;
                return;
            }

            //Finally, set LastModifierUserId!
            entityWithAudit.ModifierId = userId;
            entityWithAudit.ModifierName = userName;
        }
    }
}
