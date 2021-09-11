/**
 * 其他种类资源的工厂的接口
 * @author ligzh
 * @date 2021/6/12
 */

public interface IBaseResourceFactory<T>
{
    T GetSingleResource(string resourcePath);
}