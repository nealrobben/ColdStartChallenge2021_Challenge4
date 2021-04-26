const driverData = require('../shared/driver-data');

module.exports = async function (context, req) {
  try {
    console.log('Get drivers list');
    let items = await driverData.getDrivers();

    context.res.status(200).json(items);
  } catch (error) {
    context.res.status(500).send(error);
  }
};