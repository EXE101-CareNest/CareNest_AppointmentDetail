using CareNest_AppointmentDetail.Application.Features.Commands.Create;
using CareNest_AppointmentDetail.Application.Features.Commands.Update;
using CareNest_AppointmentDetail.Domain.Commons.Constant;

namespace CareNest_AppointmentDetail.Application.Exceptions.Validators
{
    public class Validate
    {
        /// <summary>
        /// kiểm tra toàn bộ tạo cuộc hẹn 
        /// </summary>
        /// <param name="command"></param>
        public static void ValidateCreate(CreateCommand command)
        {
            ValidateQuantity(command.PetQuantity);
        }
        /// <summary>
        /// kiểm tra cập nhật cuộc hẹn 
        /// </summary>
        /// <param name="command"></param>
        public static void ValidateUpdate(UpdateCommand command)
        {
            ValidateQuantity(command.PetQuantity);
        }

        public static void ValidateQuantity(int? quantity)
        {

            if (quantity <= 0)
            {
                throw new BadRequestException(MessageConstant.InvalidQuantity);
            }

        }

    }
}
