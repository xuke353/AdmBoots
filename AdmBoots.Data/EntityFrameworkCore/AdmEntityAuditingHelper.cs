using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure;
using AdmBoots.Infrastructure.Framework.Abstractions;
using AdmBoots.Infrastructure.Framework.Interface;
using AdmBoots.Infrastructure.Framework.Web;
using Microsoft.Extensions.DependencyInjection;

namespace AdmBoots.Data.EntityFrameworkCore {

    public class AdmEntityAuditingHelper {

        internal static void SetCreationAuditProperties(object entityAsObj, IAdmSession admSession) {
            if (entityAsObj is not CreationEntity entityWithCreationTime) {
                //Object does not implement CreationEntity
                return;
            }
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

        internal static void SetModificationAuditProperties(object entityAsObj, IAdmSession admSession) {
            if (entityAsObj is not AuditEntity entityWithAudit) {
                return;
            }
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

        internal static void SetDeletionAuditProperties(object entity, IAdmSession session) {
            //var softDelete = entity as ISoftDelete;
            //if (softDelete == null) {
            //    return;
            //}
            //上面代码和下面代码意思相同
            if (entity is not ISoftDelete softDelete) {
                return;
            }

            softDelete.DeletionTime = DateTime.Now;
            softDelete.DeleterId = session.UserId;
        }
    }
}
