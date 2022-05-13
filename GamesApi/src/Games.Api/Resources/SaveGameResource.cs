using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games.Api.Resources;

public record SaveGameResource
(
    string Name,
    string Description
);
