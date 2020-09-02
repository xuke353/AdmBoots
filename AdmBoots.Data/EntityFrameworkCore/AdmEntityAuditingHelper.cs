using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure.Framework.Abstractions;

namespace AdmBoots.Data.EntityFrameworkCore {

    public class AdmEntityAuditingHelper {

        public static void SetCreationAuditProperties(object entityAsObj, int? userId, string userName) {
            var entityWithCreationTime = entityAsObj as CreationEntity<int>;
            if (entityWithCreationTime == null) {
                //Object does not implement CreationEntity
                return;
            }

            if (entityWithCreationTime.CreateTime.GetValueOrDefault() == default(DateTime)) {
                entityWithCreationTime.CreateTime = DateTime.Now;
            }

            if (userId.HasValue || userId == 0) {
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

        public static void SetModificationAuditProperties(object entityAsObj, int? userId, string userName) {
            var entityWithAudit = entityAsObj as AuditEntity;
            if (entityWithAudit == null) {
                return;
            }

            entityWithAudit.ModifyTime = DateTime.Now;

            if (userId.HasValue || userId == 0) {
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
