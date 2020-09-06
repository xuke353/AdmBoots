using System;
using System.Collections.Generic;
using System.Text;

namespace AdmBoots.Quartz.Common {
    public enum RequestType {
        None = 0,
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8
    }

    public enum JobAction {
        新增 = 1,
        删除 = 2,
        修改 = 3,
        暂停 = 4,
        停止,
        开启,
        立即执行
    }
}
