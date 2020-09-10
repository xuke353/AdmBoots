using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace AdmBoots.Infrastructure.Mail {

    public class EmailService {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="config">邮件配置</param>
        /// <param name="mail">邮件内容</param>

        public static void PostEmail(MailConfig config, MailModel mail) {
            //创建Email的Message对象
            MailMessage mailMessage = new MailMessage {
                //发件人邮箱
                From = new MailAddress(config.Fr),
                //标题
                Subject = string.Format(mail.Title.Replace('r', ' ').Replace('n', ' ')),
                //编码
                SubjectEncoding = Encoding.UTF8,
                //正文
                Body = mail.Body,
                //正文编码
                BodyEncoding = Encoding.Default,
                //邮件优先级
                Priority = MailPriority.Normal,
                // 正文是否是html格式
                IsBodyHtml = false
            };
            //判断收件人数组中是否有数据
            if (config.ToArry.Any()) {
                //循环添加收件人地址
                foreach (var to in config.ToArry) {
                    if (!string.IsNullOrEmpty(to))
                        mailMessage.To.Add(to);
                }
            }

            //判断抄送地址数组是否有数据
            if (config.CcArray.Any()) {
                //循环添加抄送地址
                foreach (var cc in config.CcArray) {
                    if (!string.IsNullOrEmpty(cc))
                        mailMessage.CC.Add(cc);
                }
            }

            //实例化一个Smtp客户端
            SmtpClient smtp = new SmtpClient {
                //将发件人的邮件地址和客户端授权码带入以验证发件人身份
                Credentials = new System.Net.NetworkCredential(config.Fr, config.Code),
                //指定SMTP邮件服务器
                Host = config.FrHost
            };

            //邮件发送到SMTP服务器
            smtp.Send(mailMessage);
        }
    }
}
