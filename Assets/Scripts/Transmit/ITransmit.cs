/**
 * 转换状态的接口
 * @author ligzh
 * @date 2021/9/3
 */

public interface ITransmit
{
    void OnTransmit(ETransmitType type, BaseTransmitArg arg);
}